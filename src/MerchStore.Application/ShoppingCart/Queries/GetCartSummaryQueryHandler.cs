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
    private readonly IMediator _mediator;
    private readonly ILogger<GetCartSummaryQueryHandler> _logger;

    public GetCartSummaryQueryHandler(IMediator mediator, ILogger<GetCartSummaryQueryHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<CartSummaryDto> Handle(GetCartSummaryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetCartSummaryQuery for cart ID: {CartId}.", request.CartId);

        // Retrieve the cart using Mediatr
        var cart = await _mediator.Send(new GetCartQuery(request.CartId), cancellationToken);
        if (cart == null)
        {
            _logger.LogWarning("Cart with ID {CartId} not found. Returning default summary.", request.CartId);
            return new CartSummaryDto
            {
                CartId = request.CartId,
                ProductCount = 0,
                TotalPrice = new Money(0, "SEK") // Default total price
            };
        }

        // Map the cart to a CartSummaryDto
        return new CartSummaryDto
        {
            CartId = cart.CartId,
            ProductCount = cart.Products.Sum(p => p.Quantity),
            TotalPrice = new Money(cart.Products.Sum(p => p.UnitPrice.Amount * p.Quantity), "SEK")
        };
    }
}