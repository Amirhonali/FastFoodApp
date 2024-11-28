using System;
using Domain.Entities;

namespace Application.Repositories
{
    public interface IRoleRepository : IRepository<Role>
    {
        public Task<IEnumerable<Role>> GetRolesByIdsAsync(int[] roleId);
    }
}

