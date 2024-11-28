using System;
using System.Linq.Expressions;
using Application.Abstraction;
using Application.Repositories;
using Domain.Entities;

namespace Infrastructure.Services
{
    public class ProductRepository : IProductRepository
    {
        private readonly IApiDbContext _apiDbContext;

        public ProductRepository(IApiDbContext apiDbContext)
        {
            _apiDbContext = apiDbContext;
        }

        public async Task<Product> AddAsync(Product prod)
        {
            _apiDbContext.Products.Add(prod);

            var res = await _apiDbContext.SaveChangesAsync();
            if (res > 0)
            {
                return prod;
            }
            return null;
        }

        public async Task<IEnumerable<Product>?> AddRangeAsync(IEnumerable<Product> prods)
        {
            _apiDbContext.Products.AddRange(prods);

            int res = await _apiDbContext.SaveChangesAsync();

            if(res > 0) { return prods; }

            return null;
        }

        public async Task<bool> DeleteAsync(int Id)
        {
            Product? prod = await _apiDbContext.Products.FindAsync(Id);

            if(prod != null)
            {
                _apiDbContext.Products.Remove(prod);
            }
            int res = await _apiDbContext.SaveChangesAsync();
            if(res > 0)
            {
                return true;
            }
            return false;
        }

        public IEnumerable<Product> GetAsync(Expression<Func<Product, bool>> predicate)
        {
            return _apiDbContext.Products.Where(predicate).AsEnumerable();
        }

        public async Task<Product?> GetByIdAsync(int Id)
        {
            return await _apiDbContext.Products.FindAsync(Id);
        }

        public IEnumerable<Product> SearchingProducts(string text)
        {
            text = text.ToLower(); 
            return _apiDbContext.Products
                .ToList()
                .Where(x => x.Name.ToLower().Contains(text) ||
                            x.Category.ToString().ToLower().Contains(text) ||
                            x.Price.ToString().ToLower().Contains(text));
        }


        public async Task<Product> UpdateAsync(Product prod)
        {
            _apiDbContext.Products.Update(prod);

            int res = await _apiDbContext.SaveChangesAsync();
            if(res > 0) { return prod; }
            return null;
        }
    }
}

// dotnet run --urls "http://192.168.0.150:5257"