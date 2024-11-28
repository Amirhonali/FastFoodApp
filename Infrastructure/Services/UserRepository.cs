using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using Application.Abstraction;
using Application.Extentions;
using Application.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MailKit;

namespace Infrastructure.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly IApiDbContext _apiDbContext;

        public UserRepository(IApiDbContext apiDbContext)
        {
            _apiDbContext = apiDbContext;
        }

        public async Task<User> AddAsync(User user)
        {
            _apiDbContext.Users.Add(user);

            int res = await _apiDbContext.SaveChangesAsync();
            if (res > 0) return user;

            return null;
        }

        public async Task<IEnumerable<User>> AddRangeAsync(IEnumerable<User> user)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(int Id)
        {
            User? user = await _apiDbContext.Users.FindAsync(Id);

            if (user != null)
            {
                _apiDbContext.Users.Remove(user);
                int res = await _apiDbContext.SaveChangesAsync();
                if (res > 0) return true;
            }
            return false;
        }

        public IEnumerable<User> GetAsync(Expression<Func<User, bool>> predicate)
        {
            return _apiDbContext.Users.Where(predicate).Include(x => x.Roles);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _apiDbContext.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public Task<User?> GetByIdAsync(int Id)
        {
            return Task.FromResult(_apiDbContext.Users.Where(x => x.Id.Equals(Id))
                .Include(x => x.Roles)
                .SingleOrDefault());
        }

        public async Task<User> UpdateAsync(User user)
        {
            var existingUser = await GetByIdAsync(user.Id);
            if (existingUser != null)
            {
                existingUser.FullName = user.FullName;
                existingUser.Email = user.Email;
                existingUser.Roles = user.Roles;

                int res = await _apiDbContext.SaveChangesAsync();
                if (res > 0)
                {
                    return user;
                }
            }
            return null;
        }

        public async Task<User> UpdateWithVerifyAsync(User user)
        {
            var existingUser = await GetByIdAsync(user.Id);
            if (existingUser != null)
            {
                existingUser.FullName = user.FullName;
                existingUser.Email = user.Email;

                foreach (var role in user.Roles)
                {
                    var existingRoles = _apiDbContext.Roles.Find(role.RoleId);
                    if (existingRoles != null)
                    {
                        existingUser.Roles.Add(existingRoles);
                    }
                }

                int res = await _apiDbContext.SaveChangesAsync();
                if (res > 0)
                {
                    return user;
                }
            }
            return null;
        }
    }
}

