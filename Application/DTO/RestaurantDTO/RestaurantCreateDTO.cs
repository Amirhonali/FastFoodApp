using System;
using Microsoft.AspNetCore.Http;

namespace Application.DTO.RestaurantDTO
{
	public class RestaurantCreateDTO
	{
        public string Name { get; set; }

        public string Description { get; set; }

        public IFormFile Image { get; set; }

    }
}

