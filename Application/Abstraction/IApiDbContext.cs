using System;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Application.Abstraction
{
	public interface IApiDbContext
	{
		public DbSet<Restaurant> Restaurants { get; set; }

		public DbSet<Product> Products { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<RefreshToken> RefreshToken { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

