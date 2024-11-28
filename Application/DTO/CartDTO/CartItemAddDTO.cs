using System;
using Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace Application.DTO.CartDTO
{
    public class CartItemAddDTO
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }

}

