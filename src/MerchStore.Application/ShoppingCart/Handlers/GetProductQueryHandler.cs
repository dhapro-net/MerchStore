using MediatR;
using MerchStore.Application.ShoppingCart.DTOs;
using MerchStore.Application.ShoppingCart.Queries;
using Microsoft.Extensions.Logging;

namespace MerchStore.Application.ShoppingCart.Handlers;

/// <summary>
/// Handles the retrieval of a specific product from a shopping cart.
/// </summary>
public class GetProductQueryHandler : IRequestHandler<GetProductQuery, CartProductDto?>
{
    private readonly IMediator _mediator;
    private readonly ILogger<GetProductQueryHandler> _logger;

    public GetProductQueryHandler(IMediator mediator, ILogger<GetProductQueryHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<CartProductDto?> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetProductQuery for product {ProductId} in cart {CartId}.", request.ProductId, request.CartId);

        // Retrieve the cart using Mediatr
        var cart = await _mediator.Send(new GetCartQuery(request.CartId), cancellationToken);
        if (cart == null)
        {
            _logger.LogWarning("Cart with ID {CartId} not found.", request.CartId);
            return null;
        }

        // Find the product in the cart
        var product = cart.Products.FirstOrDefault(p => p.ProductId == request.ProductId);
        if (product == null)
        {
            _logger.LogWarning("Product {ProductId} not found in cart with ID {CartId}.", request.ProductId, request.CartId);
            return null;
        }

        // Map the product to a CartProductDto
        return new CartProductDto
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            UnitPrice = product.UnitPrice,
            Quantity = product.Quantity
        };
    }
}