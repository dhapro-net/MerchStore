

namespace MerchStore.Domain.ShoppingCart.Interfaces;

public interface IShoppingCartQueryRepository
{
    Task<Cart> GetCartByIdAsync(Guid cartId, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid id);
}