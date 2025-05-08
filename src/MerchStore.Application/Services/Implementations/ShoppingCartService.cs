using System;
using System.Threading.Tasks;
using MerchStore.Application.Services.Interfaces;

namespace MerchStore.Application.Services
{
    //this implementation is a placeholder for webUI test
    public class ShoppingCartService : IShoppingCartService
    {
        public Task GetCartAsync(Guid cartId)
        {
            // Return a completed task as a placeholder
            return Task.CompletedTask;
        }

        public Task<bool> AddItemToCartAsync(Guid cartId, string productId, int quantity)
        {
            // Return false as a placeholder
            return Task.FromResult(false);
        }

        public Task<bool> RemoveItemFromCartAsync(Guid cartId, string productId)
        {
            // Return false as a placeholder
            return Task.FromResult(false);
        }

        public Task<bool> UpdateItemQuantityAsync(Guid cartId, string productId, int quantity)
        {
            // Return false as a placeholder
            return Task.FromResult(false);
        }

        public Task<bool> ClearCartAsync(Guid cartId)
        {
            // Return false as a placeholder
            return Task.FromResult(false);
        }

        public Task<decimal> CalculateCartTotalAsync(Guid cartId)
        {
            // Return 0 as a placeholder
            return Task.FromResult(0m);
        }
    }
}