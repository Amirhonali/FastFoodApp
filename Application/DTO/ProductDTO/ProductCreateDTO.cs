using System;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.DTO.ProductDTO
{
	public class ProductCreateDTO
	{

        public string Name { get; set; }

        public string Description { get; set; }

        public Category Category { get; set; }

        public decimal Price { get; set; }

        public IFormFile Image { get; set; }

    }
}

