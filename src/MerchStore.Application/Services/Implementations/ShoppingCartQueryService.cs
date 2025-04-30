using MerchStore.Service.ShoppingCart;

namespace MerchStore.Application.Services
{
    public class ShoppingCartQueryService : IShoppingCartQueryService
    {
        private readonly IShoppingCartService _shoppingCartService;
        
        public ShoppingCartQueryService(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService ?? throw new ArgumentNullException(nameof(shoppingCartService));
        }
        
        public async Task GetCartAsync(Guid cartId)
        {
            var cart = await _shoppingCartService.GetCartAsync(cartId);
            if (cart == null)
            {
                return new CartDto { Id = cartId };
            }
            
            var cartDto = new CartDto
            {
                Id = cart.CartId,
                TotalPrice = cart.CalculateTotal(),
                TotalItems = cart.Items.Sum(i => i.Quantity),
                LastUpdated = cart.LastUpdated
            };
            
            foreach (var item in cart.Items)
            {
                cartDto.Items.Add(new CartItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    TotalPrice = item.Quantity * item.UnitPrice
                });
            }
            
            return cartDto;
        }
        
        public async Task GetCartSummaryAsync(Guid cartId)
        {
            var cart = await _shoppingCartService.GetCartAsync(cartId);
            if (cart == null)
            {
                return new CartSummaryDto { CartId = cartId };
            }
            
            return new CartSummaryDto
            {
                CartId = cart.CartId,
                ItemsCount = cart.Items.Sum(i => i.Quantity),
                TotalPrice = cart.CalculateTotal()
            };
        }
    }
}