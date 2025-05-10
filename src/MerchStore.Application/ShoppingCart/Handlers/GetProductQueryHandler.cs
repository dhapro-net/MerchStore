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
    private readonly ILogger<GetProductQueryHandler> _logger;

    public GetProductQueryHandler(ILogger<GetProductQueryHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<CartProductDto?> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        if (request.Cart == null)
        {
            _logger.LogWarning("GetProductQuery failed: Cart is null.");
            return Task.FromResult<CartProductDto?>(null);
        }

        _logger.LogInformation("Handling GetProductQuery for product {ProductId} in cart {CartId}.", request.ProductId, request.Cart.CartId);

        // Find the product in the cart
        var product = request.Cart.Products.FirstOrDefault(p => p.ProductId == request.ProductId);
        if (product == null)
        {
            _logger.LogWarning("Product {ProductId} not found in cart with ID {CartId}.", request.ProductId, request.Cart.CartId);
            return Task.FromResult<CartProductDto?>(null);
        }

        // Map the product to a CartProductDto
        var productDto = new CartProductDto
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            UnitPrice = product.UnitPrice,
            Quantity = product.Quantity
        };

        _logger.LogInformation("Successfully retrieved product {ProductId} from cart {CartId}.", request.ProductId, request.Cart.CartId);
        return Task.FromResult(productDto);
    }
}