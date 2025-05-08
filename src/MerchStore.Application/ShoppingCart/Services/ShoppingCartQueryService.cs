using MerchStore.Application.Services.Interfaces;
using MerchStore.Application.ShoppingCart.DTOs;
using MerchStore.Application.ShoppingCart.Interfaces;
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

    public async Task<CartDto> GetCartAsync(Guid cartId)
    {
        _logger.LogInformation($"Fetching cart with ID: {cartId}");

        var cart = await _shoppingCartService.GetCartAsync(cartId);
        if (cart == null)
        {
            _logger.LogWarning($"Cart with ID {cartId} not found.");
            return new CartDto
            {
                Id = cartId,
                Items = new List<CartItemDto>(),
                TotalPrice = 0,
                TotalItems = 0,
                LastUpdated = DateTime.UtcNow
            };
        }

        var cartDto = new CartDto
        {
            Id = cart.CartId,
            TotalPrice = cart.CalculateTotal(),
            TotalItems = cart.Items?.Sum(i => i.Quantity) ?? 0,
            LastUpdated = cart.LastUpdated,
            Items = cart.Items?.Select(item => new CartItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                UnitPrice = item.UnitPrice.Amount,
                Quantity = item.Quantity,
                TotalPrice = item.Quantity * item.UnitPrice.Amount
            }).ToList() ?? new List<CartItemDto>() // Handle null cart.Items
        };

        return cartDto;
    }

    public async Task<CartSummaryDto> GetCartSummaryAsync(Guid cartId)
    {
        _logger.LogInformation($"Fetching cart summary with ID: {cartId}");

        var cart = await _shoppingCartService.GetCartAsync(cartId);
        if (cart == null)
        {
            _logger.LogWarning($"Cart summary with ID {cartId} not found.");
            return new CartSummaryDto
            {
                CartId = cartId,
                ItemsCount = 0,
                TotalPrice = 0
            };
        }

        return new CartSummaryDto
        {
            CartId = cart.CartId,
            ItemsCount = cart.Items?.Sum(i => i.Quantity) ?? 0,
            TotalPrice = cart.CalculateTotal()
        };
    }
}}