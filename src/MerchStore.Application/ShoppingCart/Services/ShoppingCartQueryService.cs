using MerchStore.Application.Services.Interfaces;
using MerchStore.Application.ShoppingCart.DTOs;
using MerchStore.Application.ShoppingCart.Interfaces;
using MerchStore.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace MerchStore.Application.ShoppingCart.Services
{
    public class ShoppingCartQueryService : IShoppingCartQueryService
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ILogger<ShoppingCartQueryService> _logger;

        public ShoppingCartQueryService(IShoppingCartService shoppingCartService, ILogger<ShoppingCartQueryService> logger)
        {
            _shoppingCartService = shoppingCartService ?? throw new ArgumentNullException(nameof(shoppingCartService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CartDto> GetCartAsync(Guid cartId, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Fetching cart with ID: {cartId}");

            var cart = await _shoppingCartService.GetOrCreateCartAsync(cartId, cancellationToken);
            if (cart == null)
            {
                _logger.LogWarning($"Cart with ID {cartId} not found.");
                return new CartDto
                {
                    CartId = cartId,
                    Products = new List<CartProductDto>(), // Fixed: Changed 'Product' to 'Products'
                    TotalPrice = new Money(0, "SEK"), // Default Money object
                    TotalProducts = 0, // Fixed: Changed 'TotalProduct' to 'TotalProducts'
                    LastUpdated = DateTime.UtcNow
                };
            }

            var cartDto = new CartDto
            {
                CartId = cart.CartId,
                TotalPrice = cart.CalculateTotal(),
                TotalProducts = cart.Products?.Sum(i => i.Quantity) ?? 0, // Fixed: Changed 'Product' to 'Products'
                LastUpdated = cart.LastUpdated,
                Products = cart.Products?.Select(product => new CartProductDto // Fixed: Changed 'Product' to 'Products'
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    UnitPrice = product.UnitPrice,
                    Quantity = product.Quantity,
                }).ToList() ?? new List<CartProductDto>()
            };

            return cartDto;
        }

        public async Task<CartSummaryDto> GetCartSummaryAsync(Guid cartId, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Fetching cart summary with ID: {cartId}");

            var cart = await _shoppingCartService.GetOrCreateCartAsync(cartId, cancellationToken);
            if (cart == null)
            {
                _logger.LogWarning($"Cart summary with ID {cartId} not found.");
                return new CartSummaryDto
                {
                    CartId = cartId,
                    ProductCount = 0,
                    TotalPrice = new Money(0, "USD") // Default Money object
                };
            }

            return new CartSummaryDto
            {
                CartId = cart.CartId,
                ProductCount = cart.Products?.Sum(i => i.Quantity) ?? 0,
                TotalPrice = cart.CalculateTotal() // Assuming CalculateTotal() now returns Money
            };
        }
    }
}