using MerchStore.Domain.ShoppingCart;
using MerchStore.Application.ShoppingCart.Dtos;

namespace MerchStore.Application.ShoppingCart.Interfaces;
{
    public interface IShoppingCartService
    {
        Task<CartDto> GetCartAsync(Guid cartId);
        Task<bool> AddItemToCartAsync(Guid cartId, string productId, int quantity);
        Task<bool> RemoveItemFromCartAsync(Guid cartId, string productId);
        Task<bool> UpdateItemQuantityAsync(Guid cartId, string productId, int quantity);
        Task<bool> ClearCartAsync(Guid cartId);
        Task<decimal> CalculateCartTotalAsync(Guid cartId);
    }
}