using System;
using Domain.Entities;

namespace Application.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        public Task<User?> GetByEmailAsync(string email);

        public Task<User> UpdateWithVerifyAsync(User user);
    }
}

