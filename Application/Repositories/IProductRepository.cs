using System;
using Domain.Entities;

namespace Application.Repositories
{
	public interface IProductRepository : IRepository<Product>
	{
		public IEnumerable<Product> SearchingProducts(string text);
	}
}

