using System;
using Application.Abstraction;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
	public class ApiDbContext : DbContext, IApiDbContext
	{
		public ApiDbContext(DbContextOptions<ApiDbContext> options)
			: base(options)
		{

		}

        public DbSet<Restaurant> Restaurants { get; set; }

        public DbSet<Product> Products { get;  set ; }

		public DbSet<User> Users { get; set; }

		public DbSet<RefreshToken> RefreshToken { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<Cart> Carts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(option => option.Email).IsUnique();
        }
    }
}

