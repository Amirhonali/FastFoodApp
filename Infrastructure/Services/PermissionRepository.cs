using System;
using System.Linq.Expressions;
using Application.Abstraction;
using Application.Repositories;
using Domain.Entities;

namespace Infrastructure.Services
{
	public class PermissionRepository : IPermissionRepository
	{
        private readonly IApiDbContext _apiDbContext;

		public PermissionRepository(IApiDbContext apiDbContext)
		{
            _apiDbContext = apiDbContext;
		}

        public async Task<Permission> AddAsync(Permission permission)
        {
            _apiDbContext.Permissions.Add(permission);

            int res = await _apiDbContext.SaveChangesAsync();
            if (res > 0) return permission;

            return null;
        }

        public async Task<IEnumerable<Permission>> AddRangeAsync(IEnumerable<Permission> permissions)
        {
            _apiDbContext.Permissions.AddRange(permissions);

            int res = await _apiDbContext.SaveChangesAsync();
            if (res > 0) return permissions;

            return null;
        }

        public async Task<bool> DeleteAsync(int Id)
        {
            Permission? permission = await _apiDbContext.Permissions.FindAsync(Id);

            if(permission != null)
            {
                _apiDbContext.Permissions.Remove(permission);
                int res = await _apiDbContext.SaveChangesAsync();
                if (res > 0) return true;
            }
            return false;
        }

        public IEnumerable<Permission> GetAsync(Expression<Func<Permission, bool>> predicate)
        {
            return _apiDbContext.Permissions.Where(predicate).AsEnumerable();
        }

        public async Task<Permission>? GetByIdAsync(int Id)
        {
            return await _apiDbContext.Permissions.FindAsync(Id);
        }

        public async Task<Permission> UpdateAsync(Permission permission)
        {
            _apiDbContext.Permissions.Update(permission);
            int res = await _apiDbContext.SaveChangesAsync();
            if (res > 0) return permission;

            return null;
        }
    }
}

