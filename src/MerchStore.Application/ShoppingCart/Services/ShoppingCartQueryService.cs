using MerchStore.Application.Services.Interfaces;
using MerchStore.Application.ShoppingCart.Dtos;
using MerchStore.Application.ShoppingCart.DTOs;
using MerchStore.Service.ShoppingCart;



namespace MerchStore.Application.ShoppingCart.Services
{
    public class ShoppingCartQueryService : IShoppingCartQueryService
    {
        
        private readonly IShoppingCartService _shoppingCartService;
        
        public ShoppingCartQueryService(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService ?? throw new ArgumentNullException(nameof(shoppingCartService));
        }
        
        public async Task<CartDto> GetCartAsync(Guid cartId)
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
                    UnitPrice = item.UnitPrice.Amount,
                    Quantity = item.Quantity,
                    TotalPrice = item.Quantity * item.UnitPrice.Amount
                });
            }
            
            return cartDto;
        }
        public async Task<CartSummaryDto> GetCartSummaryAsync(Guid cartId)
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
