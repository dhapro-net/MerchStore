

namespace MerchStore.Domain.ShoppingCart.Interfaces;

public interface IShoppingCartCommandRepository
{
    Task AddAsync(Cart cart);
    Task UpdateAsync(Cart cart);
    Task DeleteAsync(Guid id);
}