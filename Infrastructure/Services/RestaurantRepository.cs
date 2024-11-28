using System;
using System.Linq.Expressions;
using Application.Abstraction;
using Application.Repositories;
using Domain.Entities;

namespace Infrastructure.Services
{
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly IApiDbContext _apiDbContext;

        public RestaurantRepository(IApiDbContext apiDbContext)
        {
            _apiDbContext = apiDbContext;
        }

        public async Task<Restaurant> AddAsync(Restaurant rest)
        {
            _apiDbContext.Restaurants.Add(rest);

            var res = await _apiDbContext.SaveChangesAsync();
            if (res > 0)
            {
                return rest;
            }
            return null;
        }

        public async Task<IEnumerable<Restaurant>?> AddRangeAsync(IEnumerable<Restaurant> rest)
        {
            _apiDbContext.Restaurants.AddRange(rest);

            int res = await _apiDbContext.SaveChangesAsync();

            if (res > 0) { return rest; }

            return null;
        }

        public async Task<bool> DeleteAsync(int Id)
        {
            Restaurant? rest = await _apiDbContext.Restaurants.FindAsync(Id);

            if (rest != null)
            {
                _apiDbContext.Restaurants.Remove(rest);
            }
            int res = await _apiDbContext.SaveChangesAsync();
            if (res > 0)
            {
                return true;
            }
            return false;
        }

        public IEnumerable<Restaurant> GetAsync(Expression<Func<Restaurant, bool>> predicate)
        {
            return _apiDbContext.Restaurants.Where(predicate).AsEnumerable();
        }

        public async Task<Restaurant?> GetByIdAsync(int Id)
        {
            return await _apiDbContext.Restaurants.FindAsync(Id);
        }

        public IEnumerable<Restaurant> SearchingRestaurants(string text)
        {
            text = text.ToLower();
            return _apiDbContext.Restaurants
                .Where(x => x.Name.ToLower().Contains(text))
                .ToList();
        }

        public async Task<Restaurant> UpdateAsync(Restaurant rest)
        {
            _apiDbContext.Restaurants.Update(rest);

            int res = await _apiDbContext.SaveChangesAsync();
            if (res > 0) { return rest; }
            return null;
        }
    }
}

