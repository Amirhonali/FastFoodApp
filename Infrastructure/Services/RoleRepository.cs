using System;
using System.Linq.Expressions;
using Application.Abstraction;
using Application.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Infrastructure.Services
{
	public class RoleRepository : IRoleRepository
	{

        private readonly IApiDbContext _apiDbContext;

        public RoleRepository(IApiDbContext apiDbContext)
        {
            _apiDbContext = apiDbContext;
        }

        public async Task<Role> AddAsync(Role role)
        {
            _apiDbContext.Roles.Add(role);

            int res = await _apiDbContext.SaveChangesAsync();
            if (res > 0) return role;

            return null;
        }

        public async Task<IEnumerable<Role>> AddRangeAsync(IEnumerable<Role> roles)
        {
            _apiDbContext.Roles.AddRange(roles);

            int res = await _apiDbContext.SaveChangesAsync();
            if (res > 0) return roles;

            return null;
        }

        public async Task<bool> DeleteAsync(int Id)
        {
            Role? role = await _apiDbContext.Roles.FindAsync(Id);

            if (role != null)
            {
                _apiDbContext.Roles.Remove(role);
                int res = await _apiDbContext.SaveChangesAsync();
                if (res > 0) return true;
            }
            return false;
        }

        public IEnumerable<Role> GetAsync(Expression<Func<Role, bool>> predicate)
        {
            return _apiDbContext.Roles.Where(predicate).Include(x => x.Permissions).AsEnumerable();
        }

        public async Task<Role?> GetByIdAsync(int Id)
        {
            return _apiDbContext.Roles.Where(x => x.RoleId == Id).Include(x => x.Permissions).SingleOrDefault();
        }

        public async Task<IEnumerable<Role>> GetRolesByIdsAsync(int[] roleIds)
        {
            return _apiDbContext.Roles
                .Where(role => roleIds.Contains(role.RoleId))
                .AsEnumerable(); 
        }

        public async Task<Role> UpdateAsync(Role updatedRole)
        {
            var existingRole = await GetByIdAsync(updatedRole.RoleId);
            if (existingRole != null)
            {
                existingRole.Name = updatedRole.Name;

                existingRole.Permissions.Clear();
                foreach (var permission in updatedRole.Permissions)
                {
                    var existingPermission = _apiDbContext.Permissions.Find(permission.PermissionId);
                    if (existingPermission != null)
                    {
                        existingRole.Permissions.Add(existingPermission);
                    }
                }

                int res = await _apiDbContext.SaveChangesAsync();
                if (res > 0)
                {
                    return updatedRole;
                }
            }
            return null;
        }
    }
}

