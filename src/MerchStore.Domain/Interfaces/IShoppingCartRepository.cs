


namespace MerchStore.Domain.Interfaces
{
    public interface IShoppingCartRepository
    {
        Task<ShoppingCart.ShoppingCart> GetByIdAsync(Guid id);
        Task AddAsync(ShoppingCart.ShoppingCart cart);
        Task UpdateAsync(ShoppingCart.ShoppingCart cart);
    }
}