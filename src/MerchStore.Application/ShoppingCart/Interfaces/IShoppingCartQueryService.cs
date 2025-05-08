using System;
using System.Threading.Tasks;
using MerchStore.Application.ShoppingCart.DTOs;

namespace MerchStore.Application.Services.Interfaces
{
    public interface IShoppingCartQueryService
    {
        Task<CartDto> GetCartAsync(Guid cartId);
        Task<CartSummaryDto> GetCartSummaryAsync(Guid cartId);
    }
}