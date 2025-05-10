using MediatR;
using MerchStore.Application.Common;
using MerchStore.Application.ShoppingCart.Interfaces;
using MerchStore.Domain.ShoppingCart;
using Microsoft.Extensions.Logging;

namespace MerchStore.Application.ShoppingCart.Commands;

/// <summary>
/// Handles the command to remove a product from the shopping cart.
/// </summary>
public class RemoveProductFromCartCommandHandler : IRequestHandler<RemoveProductFromCartCommand, Result<bool>>
{
    private readonly ILogger<RemoveProductFromCartCommandHandler> _logger;

    public RemoveProductFromCartCommandHandler(ILogger<RemoveProductFromCartCommandHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<Result<bool>> Handle(RemoveProductFromCartCommand request, CancellationToken cancellationToken)
    {
        if (request.Cart == null)
        {
            _logger.LogWarning("RemoveProductFromCartCommand failed: Cart is null.");
            return Task.FromResult(Result.Failure<bool>("Cart cannot be null."));
        }

        if (string.IsNullOrEmpty(request.ProductId))
        {
            _logger.LogWarning("RemoveProductFromCartCommand failed: Product ID is empty.");
            return Task.FromResult(Result.Failure<bool>("Product ID cannot be empty."));
        }

        try
        {
            _logger.LogInformation("Attempting to remove product with ID: {ProductId} from cart with ID: {CartId}", request.ProductId, request.Cart.CartId);

            // Remove the product from the cart
            var productRemoved = request.Cart.RemoveProduct(request.ProductId);
            if (!productRemoved)
            {
                _logger.LogWarning("Product with ID {ProductId} not found in cart with ID {CartId}.", request.ProductId, request.Cart.CartId);
                return Task.FromResult(Result.Failure<bool>("Product not found in the cart."));
            }

            _logger.LogInformation("Successfully removed product with ID: {ProductId} from cart with ID: {CartId}", request.ProductId, request.Cart.CartId);
            return Task.FromResult(Result.Success(true));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while removing product with ID: {ProductId} from cart with ID: {CartId}", request.ProductId, request.Cart.CartId);
            return Task.FromResult(Result.Failure<bool>("An unexpected error occurred while removing the product from the cart."));
        }
    }
}
