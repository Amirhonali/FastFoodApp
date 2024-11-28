using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
	public class Restaurant
	{
        public int Id { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

        public byte[] ImageData { get; set; }

    }
}

