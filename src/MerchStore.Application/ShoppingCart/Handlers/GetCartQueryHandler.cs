using MediatR;
using MerchStore.Application.ShoppingCart.DTOs;
using MerchStore.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
namespace MerchStore.Application.ShoppingCart.Queries;

/// <summary>
/// Handles the query to retrieve a shopping cart.
/// </summary>
public class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartDto?>
{
    private readonly ILogger<GetCartQueryHandler> _logger;

    public GetCartQueryHandler(ILogger<GetCartQueryHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<CartDto?> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        // Return null if the cart is null
        if (request.Cart == null)
        {
            _logger.LogWarning("GetCartQuery received with null Cart.");
            return Task.FromResult<CartDto?>(null);
        }

        _logger.LogInformation("Mapping cart with ID {CartId} to CartDto.", request.Cart.CartId);

        // Map Cart to CartDto
        var cartDto = new CartDto
        {
            CartId = request.Cart.CartId,
            Products = request.Cart.Products.Select(p => new CartProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                UnitPrice = p.UnitPrice,
                Quantity = p.Quantity
            }).ToList(),
            TotalPrice = new Money(request.Cart.Products.Sum(p => p.UnitPrice.Amount * p.Quantity), "SEK"),
            TotalProducts = request.Cart.Products.Sum(p => p.Quantity),
            LastUpdated = request.Cart.LastUpdated
        };

        _logger.LogInformation("Successfully mapped cart with ID {CartId} to CartDto.", request.Cart.CartId);
        return Task.FromResult<CartDto?>(cartDto);
    }
}