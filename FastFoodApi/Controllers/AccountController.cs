using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Abstraction;
using Application.DTO.UserDTO;
using Application.Extentions;
using Application.Models;
using Application.Repositories;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FastFoodApi.Controllers
{
    [ApiController]
    [Route("/")]
    public class AccountController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        private readonly IUserRepository _userRepository;

        private readonly IRoleRepository _roleRepository;

        private readonly IMapper _mapper;

        private readonly IEmailService _emailService;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(ITokenService tokenService, IUserRepository userRepository, IMapper mapper, IRoleRepository roleRepository, IEmailService emailService, IHttpContextAccessor httpContextAccessor)
        {
            _tokenService = tokenService;
            _userRepository = userRepository;
            _mapper = mapper;
            _roleRepository = roleRepository;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetUserIdFromToken()
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst("id");
            if (userIdClaim == null)
            {
                throw new Exception("User ID not found in token");
            }

            if (!int.TryParse(userIdClaim.Value, out var userId))
            {
                throw new Exception("Invalid User ID in token");
            }

            return userId;
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromForm] UserCredentails userCredentails)
        {
            var user = ( _userRepository.GetAsync(x =>
                x.Password == userCredentails.Password.GetHash() &&
                x.Email == userCredentails.Email)).FirstOrDefault();

            if (user != null)
            {
                if (!user.IsConfirmed)
                {
                    return BadRequest("Your account is not confirmed. Please confirm your account before logging in.");
                }

                UserRegisterDTO userRegisterDTO = new()
                {
                    User = user,
                    UserTokens = await _tokenService.CreateTokensAsync(user)
                };
                return Ok(userRegisterDTO);
            }

            return BadRequest("Login email or password is incorrect!");
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Register([FromBody] UserCreateDTO newUser)
        {
            User user = _mapper.Map<User>(newUser);
            user.Password = user.Password.GetHash();

            var existingUser = await _userRepository.GetByEmailAsync(newUser.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "The email address is already in use. You can login from this email.");
                return BadRequest(ModelState);
            }

            const int defaultRoleId = 1;
            var roles = (await _roleRepository.GetRolesByIdsAsync(new[] { defaultRoleId })).ToList();
            if (!roles.Any())
            {
                ModelState.AddModelError("Roles", "Invalid role ID provided.");
                return BadRequest(ModelState);
            }
            user.Roles = roles;

            var confirmationCode = new Random().Next(1000, 9999).ToString();
            user.ConfirmationCode = confirmationCode;
            user.IsConfirmed = false;

            await _emailService.SendEmailAsync(user.Email, "Подтверждение аккаунта", $"Ваш код подтверждения: {confirmationCode}");

            user = await _userRepository.AddAsync(user);
            if (user != null)
            {
                return Ok(new { Message = "Код подтверждения отправлен на ваш адрес электронной почты." });
            }

            return BadRequest(ModelState);
        }



        [HttpPost("[action]")]
        public async Task<IActionResult> VerifyConfirmationCode([FromBody] VerifyCodeDTO verifyCodeDto)
        {
            var user = await _userRepository.GetByEmailAsync(verifyCodeDto.Email);

            if (user == null)
            {
                return NotFound(new { Message = "Пользователь не найден." });
            }

            if (user.IsConfirmed)
            {
                return BadRequest(new { Message = "Аккаунт уже подтвержден." });
            }

            if (user.ConfirmationCode == verifyCodeDto.Code)
            {
                user.IsConfirmed = true;
                await _userRepository.UpdateAsync(user);


                var userDTO = new UserRegisterDTO
                {
                    UserTokens = await _tokenService.CreateTokensAsync(user)
                };

                return Ok(new
                {
                    Message = "Аккаунт успешно подтвержден.",
                    Tokens = userDTO.UserTokens
                });
            }

            return BadRequest(new { Message = "Неверный код подтверждения." });
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("[action]")]
        [Authorize(Roles = "RefreshToken")]
        public async Task<IActionResult> Refresh([FromBody] Token tokens)
        {
            var principal = _tokenService.GetClaimsFromExpiredToken(tokens.AccessToken);
            string? email = principal.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                return NotFound("Refresh token not found!");
            }
            RefreshToken? savedRefreshToken = _tokenService.Get(x => x.Email == email &&
                                                      x.RefreshTokenValue == tokens.RefreshToken)
                                                     .FirstOrDefault();

            if (savedRefreshToken == null)
            {
                return BadRequest("Refresh token or Access token invalid!");
            }
            if (savedRefreshToken.ExpiredDate < DateTime.UtcNow)
            {
                _tokenService.Delete(savedRefreshToken);
                return StatusCode(405, "Refresh token already expired please login again");
            }
            Token newTokens = await _tokenService.CreateTokensFromRefresh(principal, savedRefreshToken);

            return Ok(newTokens);

        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ForgotPassword([FromForm] string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("User with this email does not exist.");
            }

            var resetToken = new Random().Next(100000, 999999).ToString(); 
            user.PasswordResetToken = resetToken;
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1); 
            await _userRepository.UpdateAsync(user);

            await _emailService.SendEmailAsync(user.Email, "Password Reset",
                $"Use this token to reset your password: {resetToken}");

            return Ok("Password reset instructions have been sent to your email.");
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ConfirmPasswordReset(string email, string passwordResetToken, string newPassword)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return BadRequest("User not found.");

            if (user.PasswordResetToken != passwordResetToken)
                return BadRequest("Invalid or expired reset token.");

            user.Password = newPassword.GetHash();
            user.PasswordResetToken = null; 
            await _userRepository.UpdateAsync(user);

            return Ok("Password has been successfully reset.");
        }


        [HttpPost("[action]")]
        [Authorize(Roles = "UploadUserImage")]
        public async Task<IActionResult> UploadUserImage(IFormFile file)
        {
            
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            var userId = GetUserIdFromToken();

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                user.ImageData = memoryStream.ToArray(); 
            }

            await _userRepository.UpdateAsync(user); 

            return Ok("Image uploaded successfully.");
        }

        [HttpDelete("[action]")]
        [Authorize(Roles = "DeleteUserImage")]
        public async Task<IActionResult> DeleteUserImage()
        {
            var userId = GetUserIdFromToken();

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.ImageData = null; 

            await _userRepository.UpdateAsync(user); 

            return Ok("Image deleted successfully.");
        }



        [HttpGet("[action]")]
        [Authorize(Roles = "GetUserImage")]
        public async Task<IActionResult> GetUserImage()
        {
            var userId = GetUserIdFromToken();

            User user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.ImageData == null)
            {
                return NotFound("Image not found for the User.");
            }

            return File(user.ImageData, "image/png");
        }
    }
}

