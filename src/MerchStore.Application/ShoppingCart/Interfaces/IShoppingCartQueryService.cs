using MerchStore.Application.ShoppingCart.DTOs;

namespace MerchStore.Application.ShoppingCart.Interfaces;

public interface IShoppingCartQueryService
{
    Task<CartDto> GetCartAsync(Guid cartId, CancellationToken cancellationToken);
    Task<CartSummaryDto> GetCartSummaryAsync(Guid cartId, CancellationToken cancellationToken);
    Task<bool> HasProductsAsync(Guid cartId, CancellationToken cancellationToken);
    Task<int> ProductCountAsync(Guid cartId, CancellationToken cancellationToken);
    Task<bool> ContainsProductAsync(Guid cartId, string productId, CancellationToken cancellationToken);
    Task<CartProductDto?> GetProductAsync(Guid cartId, string productId, CancellationToken cancellationToken);
}