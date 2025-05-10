using MerchStore.Domain.ShoppingCart;
using MerchStore.Application.ShoppingCart.DTOs;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.Application.ShoppingCart.Interfaces
{
    public interface IShoppingCartService
    {
        Task<CartDto> GetOrCreateCartAsync(Guid cartId, CancellationToken cancellationToken); 

        Task<bool> AddProductToCartAsync(Guid cartId, string productId, int quantity, CancellationToken cancellationToken);
        Task<bool> RemoveProductFromCartAsync(Guid cartId, string productId);
        Task<bool> UpdateProductQuantityAsync(Guid cartId, string productId, int quantity);
        Task<bool> ClearCartAsync(Guid cartId);
        Task<Money> CalculateCartTotalAsync(Guid cartId);

    }
}