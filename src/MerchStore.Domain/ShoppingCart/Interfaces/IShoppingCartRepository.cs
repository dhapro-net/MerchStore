using System;
using System.Threading.Tasks;
using MerchStore.Domain.ShoppingCart;

namespace MerchStore.Domain.ShoppingCart.Interfaces
{
    public interface IShoppingCartRepository
    {
        Task<Cart> GetCartByIdAsync(Guid cartId, CancellationToken cancellationToken);
         Task AddAsync(Cart cart);
        Task UpdateAsync(Cart cart);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}