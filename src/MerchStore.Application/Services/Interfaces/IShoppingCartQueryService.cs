using System;
using System.Threading.Tasks;
using MerchStore.Application.ShoppingCart.Dtos;

namespace MerchStore.Application.ShoppingCart.Interfaces
{
    public interface IShoppingCartQueryService
    {
        Task GetCartAsync(Guid cartId);
        Task GetCartSummaryAsync(Guid cartId);
    }
}