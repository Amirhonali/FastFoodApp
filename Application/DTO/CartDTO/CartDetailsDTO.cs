using System.ComponentModel.DataAnnotations;
using Application.DTO.CartDTO;

namespace Application.DTOs
{
    public class CartDetailsDTO
    {
        public int Id { get; set; }

        public decimal TotalPrice { get; set; }

        public List<CartItemDTO> Items { get; set; }
    }
}
