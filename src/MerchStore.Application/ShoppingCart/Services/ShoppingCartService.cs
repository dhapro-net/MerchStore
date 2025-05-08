using MerchStore.Application.Services.Interfaces;
using MerchStore.Application.ShoppingCart.Interfaces;
using MerchStore.Service.ShoppingCart;



namespace MerchStore.Application.Service.ShoppingCart
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductCatalogService _productCatalogService;

        public ShoppingCartService(ICartRepository cartRepository, IProductCatalogService productCatalogService)
        {
            _cartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
            _productCatalogService = productCatalogService ?? throw new ArgumentNullException(nameof(productCatalogService));
        }
        
       public async Task<Domain.ShoppingCart.Cart> GetCartAsync(Guid cartId)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId);
            if (cart == null)
            {
                cart = Domain.ShoppingCart.Cart.Create(cartId);
                await _cartRepository.AddAsync(cart);
            }
            return cart;
        }
        
        public async Task<bool> AddItemToCartAsync(Guid cartId, string productId, int quantity)
        {
            // Return false as a placeholder
            return false;
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

        public async Task<decimal> CalculateCartTotalAsync(Guid cartId)
        {
            var cart = await GetCartAsync(cartId);
            return cart.CalculateTotal();
        }
    }
}