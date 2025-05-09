using MediatR;
using MerchStore.Application.Common;
using MerchStore.Application.ShoppingCart.Interfaces;
using Microsoft.Extensions.Logging;

namespace MerchStore.Application.ShoppingCart.Commands;

/// <summary>
/// Handles the command to update the quantity of a product in the shopping cart.
/// </summary>
public class UpdateCartProductQuantityCommandHandler : IRequestHandler<UpdateCartProductQuantityCommand, Result<bool>>
{
    private readonly IShoppingCartCommandService _cartService;
    private readonly ILogger<UpdateCartProductQuantityCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateCartProductQuantityCommandHandler"/> class.
    /// </summary>
    /// <param name="cartService">The shopping cart service.</param>
    /// <param name="logger">The logger for logging operations.</param>
    public UpdateCartProductQuantityCommandHandler(IShoppingCartCommandService cartService, ILogger<UpdateCartProductQuantityCommandHandler> logger)
    {
        _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the command to update the quantity of a product in the shopping cart.
    /// </summary>
    /// <param name="request">The command containing the cart ID, product ID, and new quantity.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result indicating whether the operation was successful.</returns>
    public async Task<Result<bool>> Handle(UpdateCartProductQuantityCommand request, CancellationToken cancellationToken)
    {
        if (request.CartId == Guid.Empty)
        {
            _logger.LogWarning("UpdateCartProductQuantityCommand failed: Cart ID is empty.");
            return Result.Failure<bool>("Cart ID cannot be empty.");
        }

        if (string.IsNullOrEmpty(request.ProductId))
        {
            _logger.LogWarning("UpdateCartProductQuantityCommand failed: Product ID is empty.");
            return Result.Failure<bool>("Product ID cannot be empty.");
        }

        if (request.Quantity <= 0)
        {
            _logger.LogWarning("UpdateCartProductQuantityCommand failed: Quantity is less than or equal to zero.");
            return Result.Failure<bool>("Quantity must be greater than zero.");
        }

        _logger.LogInformation($"Updating product quantity in cart. Cart ID: {request.CartId}, Product ID: {request.ProductId}, Quantity: {request.Quantity}");

        var success = await _cartService.UpdateProductQuantityAsync(request.CartId, request.ProductId, request.Quantity);

        if (!success)
        {
            _logger.LogError($"Failed to update product quantity in cart. Cart ID: {request.CartId}, Product ID: {request.ProductId}");
            return Result.Failure<bool>("Failed to update product quantity in the shopping cart.");
        }

        _logger.LogInformation($"Successfully updated product quantity in cart. Cart ID: {request.CartId}, Product ID: {request.ProductId}");
        return Result.Success(true);
    }
}