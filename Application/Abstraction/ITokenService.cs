using System;
using Application.Models;
using System.Linq.Expressions;
using System.Security.Claims;
using Domain.Entities;

namespace Application.Abstraction
{
	public interface ITokenService
	{
        public Task<Token> CreateTokensAsync(User user);

        public Task<Token> CreateTokensFromRefresh(ClaimsPrincipal principal, RefreshToken savedRefreshToken);

        public ClaimsPrincipal GetClaimsFromExpiredToken(string token);

        public Task<bool> AddRefreshToken(RefreshToken refreshToken);

        public bool Update(RefreshToken refreshToken);

        public IQueryable<RefreshToken> Get(Expression<Func<RefreshToken, bool>> predicate);

        public bool Delete(RefreshToken token);
    }
}

