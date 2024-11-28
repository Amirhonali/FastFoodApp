using System;
using Domain.Enums;

namespace Application.DTO.ProductDTO
{
	public class ProductUpdateDTO
	{
        public string Name { get; set; }

        public string Description { get; set; }

        public Category Category { get; set; } 

        public decimal Price { get; set; }
    }
}

