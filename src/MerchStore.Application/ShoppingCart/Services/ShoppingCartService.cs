
using MerchStore.Service.ShoppingCart;
using MerchStore.Service.Products;
using MerchStore.Domain.Interfaces;



namespace MerchStore.Application.Service.ShoppingCart
{
    public class ShoppingCartService : IShoppingCartService
    {

        public Task GetCartAsync(Guid cartId)
        {

        
            _cartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
            _productCatalogService = productCatalogService ?? throw new ArgumentNullException(nameof(productCatalogService));
        }
        
       public async Task<Domain.ShoppingCart.ShoppingCart> GetCartAsync(Guid cartId)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId);
            if (cart == null)
            {
                cart = Domain.ShoppingCart.ShoppingCart.Create(cartId);
                await _cartRepository.AddAsync(cart);
            }
            return cart;
        }
        
        public async Task<bool> AddItemToCartAsync(Guid cartId, string productId, int quantity)
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
            
            var cart = await GetCartAsync(cartId);
            return cart.CalculateTotal();
        }

        Task IShoppingCartService.GetCartAsync(Guid cartId)
        {
            return GetCartAsync(cartId);
        }
    }
}