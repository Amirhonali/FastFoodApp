using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using Application.Abstraction;
using Application.Extentions;
using Application.Models;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services
{
	public class TokenService : ITokenService
	{

		private readonly IConfiguration _configuration;

        private readonly IApiDbContext _apiDbContext;

        private readonly int _refreshTokenLifetime;

        private readonly int _accessTokenLifetime;

        public TokenService(IConfiguration configuration, IApiDbContext apiDbContext)
        {
            _configuration = configuration;
            _apiDbContext = apiDbContext;
            _refreshTokenLifetime = int.Parse(configuration["JWT:RefreshTokenLifetime"]);
            _accessTokenLifetime = int.Parse(configuration["JWT:AccessTokenLifetime"]);
        }

        public async Task<Token> CreateTokensAsync(User user)
        {
            List<Claim> claims = new()
            {
                new Claim("id", user.Id.ToString()), 
                new Claim(ClaimTypes.Name, user.Email) 
            };


            Role? role1;
            List<string> permissions = new();
            foreach(Role role in user.Roles)
            {
                role1 = _apiDbContext.Roles.Where(x => x.RoleId.Equals(role.RoleId)).Include(x => x.Permissions).SingleOrDefault();
                foreach(Permission permission in role1.Permissions)
                {
                    if (!permissions.Contains(permission?.PermissionName))
                    {
                        permissions.Add(permission?.PermissionName);
                        claims.Add(new Claim(ClaimTypes.Role, permission?.PermissionName));
                    }
                }
            }

            Token tokens = CreateToken(claims);

            RefreshToken? SavedRefreshToken = Get(x => x.Email == user.Email).FirstOrDefault();
            if(SavedRefreshToken == null)
            {
                var refreshToken = new RefreshToken()
                {
                    ExpiredDate = DateTime.UtcNow.AddMinutes(_refreshTokenLifetime),
                    RefreshTokenValue = tokens.RefreshToken,
                    Email = user.Email
                };
                await AddRefreshToken(refreshToken);
            }
            else
            {
                SavedRefreshToken.RefreshTokenValue = tokens.RefreshToken;
                SavedRefreshToken.ExpiredDate = DateTime.UtcNow.AddMinutes(_refreshTokenLifetime);
                Update(SavedRefreshToken);
            }
            return tokens;
        }

        public Task<Token> CreateTokensFromRefresh(ClaimsPrincipal principal, RefreshToken savedRefreshToken)
        {
            Token tokens = CreateToken(principal.Claims);

            savedRefreshToken.RefreshTokenValue = tokens.RefreshToken;
            savedRefreshToken.ExpiredDate = DateTime.UtcNow.AddMinutes(_refreshTokenLifetime);

            Update(savedRefreshToken);
            return Task.FromResult(tokens);
        }

        public ClaimsPrincipal GetClaimsFromExpiredToken(string token)
        {
            byte[] Key = Encoding.UTF8.GetBytes(_configuration["JWT:AudienceKey"]);

            var tokenParams = new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidAudience = _configuration["JWT:AudienceKey"],
                ValidateIssuer = true,
                ValidateLifetime = false,
                ValidIssuer = _configuration["JWT:IssuerKey"],
                IssuerSigningKey = new SymmetricSecurityKey(Key),
                ClockSkew = TimeSpan.Zero
            };

            JwtSecurityTokenHandler tokenHandler = new();
            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenParams, out SecurityToken securityToken);
            JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;

            if(jwtSecurityToken == null)
            {
                throw new SecurityTokenException("invalid token");
            }
            return principal;
        }

        public async Task<bool> AddRefreshToken(RefreshToken refreshToken)
        {
            await _apiDbContext.RefreshToken.AddAsync(refreshToken);
            await _apiDbContext.SaveChangesAsync();
            return true;
        }

        public bool Update(RefreshToken refreshToken)
        {
            _apiDbContext.RefreshToken.Update(refreshToken);
            _apiDbContext.SaveChangesAsync();
            return true;
        }

        public IQueryable<RefreshToken> Get(Expression<Func<RefreshToken, bool>> predicate)
        {
            return _apiDbContext.RefreshToken.Where(predicate);
        }

        public bool Delete(RefreshToken refreshToken)
        {
            _apiDbContext.RefreshToken.Remove(refreshToken);
            _apiDbContext.SaveChangesAsync();
            return true;
        }

        private Token CreateToken(IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:IssuerKey"],
                audience: _configuration["JWT:AudienceKey"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["JWT:AccessTokenLifetime"])),
                signingCredentials: new SigningCredentials(
                                    new SymmetricSecurityKey(
                                        Encoding.UTF8.GetBytes(_configuration["JWT:Key"])),
                                        SecurityAlgorithms.HmacSha256Signature));

            Token tokens = new()
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = GenerateRefreshToken()
            };

            return tokens;
        }

        private string GenerateRefreshToken()
        {
            return (DateTime.UtcNow.ToString() + _configuration["JWT:Key"]).GetHash();
        }
    }
}

