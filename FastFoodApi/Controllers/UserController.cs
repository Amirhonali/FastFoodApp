using System;
using Application.DTO.UserDTO;
using Application.Extentions;
using Application.Repositories;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastFoodApi.Controllers
{
    [ApiController]
    [Route("/")]
    [Authorize]
    public class UserController : ControllerBase
	{
        
        private readonly IUserRepository _userRepository;

        private readonly IRoleRepository _roleRepository;

        private readonly IMapper _mapper;

        public UserController(IRoleRepository roleRepository, IUserRepository userRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet("[action]")]
        [Authorize(Roles = "GetUserById")]
        public async Task<IActionResult> GetUserById([FromQuery] int id)
        {
            User user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound($"User Id:{id} not found!");
            }
            return Ok(user);
        }

        [HttpGet("[action]")]
        [Authorize(Roles = "GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            IEnumerable<User> Users = _userRepository.GetAsync(x => true);

            return Ok(Users);
        }

        [HttpPut("[action]")]
        [Authorize(Roles = "UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDTO userUpdateDto)
        {
            User user = _mapper.Map<User>(userUpdateDto);

            user = await _userRepository.UpdateAsync(user);
            if (user == null) return BadRequest(ModelState);

            UserGetDTO userGet = _mapper.Map<UserGetDTO>(user);
            return Ok(userGet);


        }

        [HttpDelete("[action]")]
        [Authorize(Roles = "DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromQuery] int id)
        {
            bool isDelete = await _userRepository.DeleteAsync(id);
            return isDelete ? Ok("Deleted successfully")
                : BadRequest("Delete operation failed");
        }

        [HttpPut("[action]")]
        [Authorize(Roles = "ChangeUserPassword")]
        public async Task<IActionResult> ChangeUserPassword(UserChangePasswordDTO userChangePassword)
        {
            var user = await _userRepository.GetByIdAsync(userChangePassword.UserId);

            if (user != null)
            {
                string CurrentHash = userChangePassword.CurrentPassword.GetHash();
                if (CurrentHash == user.Password
                    && userChangePassword.NewPassword == userChangePassword.ConfirmNewPassword)
                {
                    user.Password = userChangePassword.NewPassword.GetHash();
                    await _userRepository.UpdateAsync(user);
                    return Ok();
                }
                else return BadRequest("Incorrect password");
            }
            return BadRequest("User not found");

        }
    }
}

