using System;
using System.Threading.Tasks;
using MerchStore.Domain.ShoppingCart;

namespace MerchStore.Domain.ShoppingCart.Interfaces
{
    public interface IShoppingCartRepository
    {
        Task<Cart> GetByIdAsync(Guid id);
        Task AddAsync(Cart cart);
        Task UpdateAsync(Cart cart);
        Task DeleteAsync(Guid id);
        
        // Optional additional methods
        Task<bool> ExistsAsync(Guid id);
        Task<Cart> GetOrCreateCartAsync(Guid id);
    }
}