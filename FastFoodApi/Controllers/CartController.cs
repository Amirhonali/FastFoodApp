using Application.DTO.CartDTO;
using Application.DTOs;
using Application.Repositories;
using Application.Repositories;
using Domain.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FastFoodApi.Controllers
{
    [ApiController]
    [Route("/")]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;

        private readonly IProductRepository _productRepository;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartController(ICartRepository cartRepository, IHttpContextAccessor httpContextAccessor, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _httpContextAccessor = httpContextAccessor;
            _productRepository = productRepository;
        }

        private int GetUserIdFromToken()
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst("id");
            if (userIdClaim == null)
            {
                throw new Exception("User ID not found in token");
            }

            if (!int.TryParse(userIdClaim.Value, out var userId))
            {
                throw new Exception("Invalid User ID in token");
            }

            return userId;
        }


        [HttpPost("[action]")]
        [Authorize(Roles = "CreateNewCart")]
        public async Task<IActionResult> CreateNewCart()
        {
            var userId = GetUserIdFromToken();
            var newCart = await _cartRepository.CreateNewCartAsync(userId);
            return Ok(new { CartId = newCart.Id, Message = "New cart created successfully!" });
        }

        [HttpPost("[action]")]
        [Authorize(Roles = "AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] CartItemAddDTO cartItemAddDTO)
        {
            var userId = GetUserIdFromToken();
            var activeCart = (await _cartRepository.GetTotalCartsAsync(userId)).FirstOrDefault();

            if (activeCart == null)
                return BadRequest("No active cart found. Create a new cart first.");

            await _cartRepository.AddToCartAsync(activeCart.Id, cartItemAddDTO.ProductId, cartItemAddDTO.Quantity);
            return Ok("Product added to cart successfully.");
        }

        [HttpDelete("[action]")]
        [Authorize(Roles = "RemoveProductInCart")]
        public async Task<IActionResult> RemoveProductInCart(int productId)
        {
            var userId = GetUserIdFromToken();
            var activeCart = (await _cartRepository.GetTotalCartsAsync(userId)).FirstOrDefault();

            if (activeCart == null)
                return BadRequest("No active cart found.");

            await _cartRepository.RemoveProductInCartAsync(activeCart.Id, productId);
            return Ok("Product removed from cart successfully.");
        }

        [HttpDelete("[action]")]
        [Authorize(Roles = "DeleteCart")]
        public async Task<IActionResult> DeleteCart(int cartId)
        {
            var userId = GetUserIdFromToken();
            var activeCart = (await _cartRepository.GetTotalCartsAsync(userId)).FirstOrDefault(c => c.Id == cartId);

            if (activeCart == null)
                return BadRequest("Cart not found or already deleted.");

            await _cartRepository.DeleteCartAsync(cartId);
            return Ok("Cart deleted successfully.");
        }

        [HttpPut("[action]")]
        [Authorize(Roles = "UpdateQuantity")]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            var userId = GetUserIdFromToken();
            var activeCart = (await _cartRepository.GetTotalCartsAsync(userId)).FirstOrDefault();

            if (activeCart == null)
                return BadRequest("No active cart found.");

            await _cartRepository.UpdateQuantityAsync(activeCart.Id, productId, quantity);
            return Ok("Product quantity updated successfully.");
        }

        [HttpGet("[action]")]
        [Authorize(Roles = "GetTotalCarts")]
        public async Task<IActionResult> GetTotalCarts()
        {
            var userId = GetUserIdFromToken();
            var carts = await _cartRepository.GetTotalCartsAsync(userId);

            var result = carts.Select(cart => new CartDetailsDTO
            {
                Id = cart.Id,
                TotalPrice = cart.TotalPrice,
                Items = cart.CartItems.Select(item => new CartItemDTO
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToList()
            });

            return Ok(result);
        }

        [HttpGet("[action]")]
        [Authorize(Roles = "GetMyOrders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = GetUserIdFromToken();
            var orders = await _cartRepository.GetMyOrdersAsync(userId);

            var result = orders.Select(order => new
            {
                OrderId = order.Id,
                OrderDate = order.CreatedAt,
                TotalPrice = order.TotalPrice,
                Items = order.CartItems.Select(item => new CartItemDTO
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToList()
            });

            return Ok(result);
        }

        [HttpPost("[action]")]
        [Authorize(Roles = "PlaceOrder")]
        public async Task<IActionResult> PlaceOrder(int cartId, [FromBody] string location)
        {
            var userId = GetUserIdFromToken();
            var activeCart = (await _cartRepository.GetTotalCartsAsync(userId)).FirstOrDefault(c => c.Id == cartId);

            if (activeCart == null)
                return BadRequest("Cart not found or already ordered.");

            await _cartRepository.PlaceOrderAsync(cartId, location);
            return Ok("Order placed successfully.");
        }

        [HttpGet("[action]")]
        [Authorize(Roles = "GetImageCartByProductId")]
        public async Task<IActionResult> GetImageCartByProductId(int id)
        {
            Product product = await _productRepository.GetByIdAsync(id);
            if (product == null || product.ImageData == null)
            {
                return NotFound("Image not found for the product.");
            }

            return File(product.ImageData, "image/png");
        }
    }

}


