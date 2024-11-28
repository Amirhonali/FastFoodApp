using System;
using Domain.Enums;

namespace Application.DTO.ProductDTO
{
	public class ProductGetDTO
	{
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Category Category { get; set; } 
    }
}

