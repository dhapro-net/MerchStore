using MerchStore.Service.ShoppingCart;
using MerchStore.Service.Products;
using MerchStore.Domain.Interfaces;


namespace MerchStore.Application.Service.ShoppingCart
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly IProductCatalogService _productCatalogService;
        
        public ShoppingCartService(
            IShoppingCartRepository cartRepository,
            IProductCatalogService productCatalogService)
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
            // Validate product exists and is available
            var product = await _productCatalogService.GetProductByIdAsync(productId);
            if (product == null || !product.IsAvailable)
            {
                return false;
            }
            
            var cart = await GetCartAsync(cartId);
            cart.AddItem(productId, product.Name, product.Price, quantity);
            
            await _cartRepository.UpdateAsync(cart);
            return true;
        }
        
        public async Task<bool> RemoveItemFromCartAsync(Guid cartId, string productId)
        {
            var cart = await GetCartAsync(cartId);
            if (cart == null)
            {
                return false;
            }
            
            cart.RemoveItem(productId);
            await _cartRepository.UpdateAsync(cart);
            return true;
        }
        
        public async Task<bool> UpdateItemQuantityAsync(Guid cartId, string productId, int quantity)
        {
            if (quantity <= 0)
            {
                return await RemoveItemFromCartAsync(cartId, productId);
            }
            
            var cart = await GetCartAsync(cartId);
            if (cart == null)
            {
                return false;
            }
            
            // Validate product still exists
            var product = await _productCatalogService.GetProductByIdAsync(productId);
            if (product == null || !product.IsAvailable)
            {
                return false;
            }
            
            cart.UpdateQuantity(productId, quantity);
            await _cartRepository.UpdateAsync(cart);
            return true;
        }
        
        public async Task<bool> ClearCartAsync(Guid cartId)
        {
            var cart = await GetCartAsync(cartId);
            if (cart == null)
            {
                return false;
            }
            
            cart.Clear();
            await _cartRepository.UpdateAsync(cart);
            return true;
        }
        
        public async Task<decimal> CalculateCartTotalAsync(Guid cartId)
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