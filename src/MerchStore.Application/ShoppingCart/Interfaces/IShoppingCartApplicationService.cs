
using MerchStore.Application.ShoppingCart.Commands;
using MerchStore.Application.ShoppingCart.Queries;

namespace MerchStore.Application.Services.Interfaces
{
    public interface IShoppingCartApplicationService
    {
        Task GetCartAsync(GetCartQuery query);
        Task GetCartSummaryAsync(GetCartSummaryQuery query);
        Task AddItemToCartAsync(AddItemToCartCommand command);
        Task RemoveItemFromCartAsync(RemoveItemFromCartCommand command);
        Task UpdateCartItemQuantityAsync(UpdateCartItemQuantityCommand command);
        Task ClearCartAsync(ClearCartCommand command);
    }
}