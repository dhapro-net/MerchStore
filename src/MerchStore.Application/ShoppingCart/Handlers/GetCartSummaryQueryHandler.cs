using MediatR;
using MerchStore.Application.ShoppingCart.DTOs;
using MerchStore.Application.ShoppingCart.Queries;
using MerchStore.Domain.ValueObjects;
using Microsoft.Extensions.Logging;


namespace MerchStore.Application.ShoppingCart.Handlers;

/// <summary>
/// Handles the retrieval of a shopping cart summary.
/// </summary>
public class GetCartSummaryQueryHandler : IRequestHandler<GetCartSummaryQuery, CartSummaryDto>
{
    private readonly ILogger<GetCartSummaryQueryHandler> _logger;

    public GetCartSummaryQueryHandler(ILogger<GetCartSummaryQueryHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<CartSummaryDto> Handle(GetCartSummaryQuery request, CancellationToken cancellationToken)
    {
        if (request.Cart == null)
        {
            _logger.LogWarning("GetCartSummaryQuery failed: Cart is null.");
            return Task.FromResult(new CartSummaryDto
            {
                CartId = request.CartId,
                ProductCount = 0,
                TotalPrice = new Money(0, "SEK") // Default total price
            });
        }

        _logger.LogInformation("Mapping cart with ID {CartId} to CartSummaryDto.", request.Cart.CartId);

        // Map the cart to a CartSummaryDto
        var cartSummary = new CartSummaryDto
        {
            CartId = request.Cart.CartId,
            ProductCount = request.Cart.Products.Sum(p => p.Quantity),
            TotalPrice = new Money(request.Cart.Products.Sum(p => p.UnitPrice.Amount * p.Quantity), "SEK")
        };

        _logger.LogInformation("Successfully mapped cart with ID {CartId} to CartSummaryDto.", request.Cart.CartId);
        return Task.FromResult(cartSummary);
    }
}