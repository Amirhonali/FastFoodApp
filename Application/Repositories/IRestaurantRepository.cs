using System;
using Domain.Entities;

namespace Application.Repositories
{
	public interface IRestaurantRepository : IRepository<Restaurant>
	{
        public IEnumerable<Restaurant> SearchingRestaurants(string text);
    }
}

