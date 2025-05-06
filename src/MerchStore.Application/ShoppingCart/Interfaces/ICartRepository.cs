using MerchStore.Domain.ShoppingCart;

namespace MerchStore.Application.ShoppingCart.Interfaces
{
    public interface ICartRepository
    {
        Task<CartItem> GetByIdAsync(Guid id);
        Task<CartItem> CreateAsync(CartItem cart);
        Task UpdateAsync(CartItem cart);
        Task DeleteAsync(Guid id);
    }
}