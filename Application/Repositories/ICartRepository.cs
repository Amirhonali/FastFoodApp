using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Repositories
{
    public interface ICartRepository
    {
        Task<Cart> CreateNewCartAsync(int userId);

        Task AddToCartAsync(int cartId, int productId, int quantity);

        Task RemoveProductInCartAsync(int cartId, int productId);

        Task DeleteCartAsync(int cartId);
        Task UpdateQuantityAsync(int cartId, int productId, int quantity);

        Task<Cart> GetCartDetailsAsync(int cartId);

        Task<IEnumerable<Cart>> GetTotalCartsAsync(int userId);

        Task<IEnumerable<Cart>> GetMyOrdersAsync(int userId);

        Task PlaceOrderAsync(int cartId, string location);

        Task<Cart?> GetCartByIdAsync(int cartId);
    }

}
