using MediatR;
using MerchStore.Application.Common;
using Microsoft.Extensions.Logging;

namespace MerchStore.Application.ShoppingCart.Commands;

/// <summary>
/// Handles the command to update the quantity of a product in the shopping cart.
/// </summary>
public class UpdateCartProductQuantityCommandHandler : IRequestHandler<UpdateCartProductQuantityCommand, Result<bool>>
{
    private readonly ILogger<UpdateCartProductQuantityCommandHandler> _logger;

    public UpdateCartProductQuantityCommandHandler(ILogger<UpdateCartProductQuantityCommandHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<Result<bool>> Handle(UpdateCartProductQuantityCommand request, CancellationToken cancellationToken)
    {
        if (request.Cart == null)
        {
            _logger.LogWarning("UpdateCartProductQuantityCommand failed: Cart is null.");
            return Task.FromResult(Result.Failure<bool>("Cart cannot be null."));
        }

        if (string.IsNullOrEmpty(request.ProductId))
        {
            _logger.LogWarning("UpdateCartProductQuantityCommand failed: Product ID is empty.");
            return Task.FromResult(Result.Failure<bool>("Product ID cannot be empty."));
        }

        if (request.Quantity <= 0)
        {
            _logger.LogWarning("UpdateCartProductQuantityCommand failed: Quantity is less than or equal to zero.");
            return Task.FromResult(Result.Failure<bool>("Quantity must be greater than zero."));
        }

        try
        {
            _logger.LogInformation("Updating product quantity in cart. Cart ID: {CartId}, Product ID: {ProductId}, Quantity: {Quantity}", request.Cart.CartId, request.ProductId, request.Quantity);

            // Update the product quantity in the cart
            var productUpdated = request.Cart.UpdateQuantity(request.ProductId, request.Quantity);
            if (!productUpdated)
            {
                _logger.LogWarning("Product with ID {ProductId} not found in cart with ID {CartId}.", request.ProductId, request.Cart.CartId);
                return Task.FromResult(Result.Failure<bool>("Product not found in the cart."));
            }

            _logger.LogInformation("Successfully updated product quantity in cart. Cart ID: {CartId}, Product ID: {ProductId}, Quantity: {Quantity}", request.Cart.CartId, request.ProductId, request.Quantity);
            return Task.FromResult(Result.Success(true));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating product quantity in cart. Cart ID: {CartId}, Product ID: {ProductId}", request.Cart.CartId, request.ProductId);
            return Task.FromResult(Result.Failure<bool>("An unexpected error occurred while updating the product quantity in the cart."));
        }
    }
}