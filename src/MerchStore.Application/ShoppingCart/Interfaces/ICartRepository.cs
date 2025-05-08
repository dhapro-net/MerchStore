using System;
using System.Threading.Tasks;
using MerchStore.Domain.ShoppingCart;

namespace MerchStore.Application.ShoppingCart.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart> GetByIdAsync(Guid cartId);
        Task AddAsync(Cart cart);
        Task UpdateAsync(Cart cart);
        Task DeleteAsync(Guid cartId);
    }
}
