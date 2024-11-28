using Application.Abstraction;
using Application.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class CartRepository : ICartRepository
    {
        private readonly IApiDbContext _context;

        public CartRepository(IApiDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> CreateNewCartAsync(int userId)
        {
            var cart = new Cart
            {
                UserId = userId,
                TotalPrice = 0,
                IsOrdered = false
            };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task AddToCartAsync(int cartId, int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new Exception("Product not found");

            var cartItem = new CartItem
            {
                CartId = cartId,
                ProductId = productId,
                Quantity = quantity,
                Price = product.Price * quantity
            };

            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            var cart = await _context.Carts.FindAsync(cartId);
            cart.TotalPrice += cartItem.Price;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveProductInCartAsync(int cartId, int productId)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);

            if (cartItem == null)
                throw new Exception("Cart item not found");

            var cart = await _context.Carts.FindAsync(cartId);
            cart.TotalPrice -= cartItem.Price;

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCartAsync(int cartId)
        {
            var cart = await _context.Carts.Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart == null)
                throw new Exception("Cart not found");

            _context.CartItems.RemoveRange(cart.CartItems);
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuantityAsync(int cartId, int productId, int quantity)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);

            if (cartItem == null)
                throw new Exception("Cart item not found");

            var cart = await _context.Carts.FindAsync(cartId);

            if (cart == null)
                throw new Exception("Cart not found");

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new Exception("Product not found");

            cart.TotalPrice -= cartItem.Price;  

            cartItem.Quantity = quantity;
            cartItem.Price = quantity * product.Price; 

            cart.TotalPrice += cartItem.Price;  

            await _context.SaveChangesAsync();
        }

        public async Task<Cart> GetCartDetailsAsync(int cartId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Id == cartId);
        }

        public async Task<IEnumerable<Cart>> GetTotalCartsAsync(int userId)
        {
            return await _context.Carts
                .Where(c => c.UserId == userId && !c.IsOrdered)
                .Include(c => c.CartItems)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cart>> GetMyOrdersAsync(int userId)
        {
            return await _context.Carts
                .Where(c => c.UserId == userId && c.IsOrdered)
                .Include(c => c.CartItems)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task PlaceOrderAsync(int cartId, string location)
        {
            var cart = await _context.Carts.FindAsync(cartId);
            if (cart == null || cart.IsOrdered)
                throw new Exception("Invalid cart");

            cart.IsOrdered = true;

            await _context.SaveChangesAsync();
        }

        public async Task<Cart?> GetCartByIdAsync(int cartId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Id == cartId);
        }
    }
}
