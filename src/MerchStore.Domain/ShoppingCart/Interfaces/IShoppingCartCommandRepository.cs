

namespace MerchStore.Domain.ShoppingCart.Interfaces;

public interface IShoppingCartCommandRepository
{
    Task AddAsync(Cart cart, CancellationToken cancellationToken);
    Task UpdateAsync(Cart cart);
    Task DeleteAsync(Guid id);

    
}