
using MerchStore.Application.ShoppingCart.Commands;
using MerchStore.Application.ShoppingCart.Queries;

namespace MerchStore.Application.Services.Interfaces
{
    public interface IShoppingCartApplicationService
    {
        Task GetCartAsync(GetCartQuery query);
        Task GetCartSummaryAsync(GetCartSummaryQuery query);
        Task AddProductToCartAsync(AddProductToCartCommand command);
        Task RemoveProductFromCartAsync(RemoveProductFromCartCommand command);
        Task UpdateCartProductQuantityAsync(UpdateCartProductQuantityCommand command);
        Task ClearCartAsync(ClearCartCommand command);
    }
}