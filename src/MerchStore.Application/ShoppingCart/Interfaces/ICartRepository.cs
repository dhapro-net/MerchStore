using System;
using System.Threading.Tasks;
using MerchStore.Domain.ShoppingCart;

namespace MerchStore.Application.ShoppingCart.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart> GetByIdAsync(Guid id);
        Task<Cart> CreateAsync(Cart cart);
        Task UpdateAsync(Cart cart);
        Task DeleteAsync(Guid id);
    }
}