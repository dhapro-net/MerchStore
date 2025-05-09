using MediatR;
using MerchStore.Application.Common;
using MerchStore.Application.ShoppingCart.Interfaces;
using Microsoft.Extensions.Logging;

namespace MerchStore.Application.ShoppingCart.Commands;

/// <summary>
/// Handles the command to remove a product from the shopping cart.
/// </summary>
public class RemoveProductFromCartCommandHandler : IRequestHandler<RemoveProductFromCartCommand, Result<bool>>
{
    private readonly IShoppingCartCommandService _cartService;
    private readonly ILogger<RemoveProductFromCartCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveProductFromCartCommandHandler"/> class.
    /// </summary>
    /// <param name="cartService">The shopping cart service.</param>
    /// <param name="logger">The logger for logging operations.</param>
    public RemoveProductFromCartCommandHandler(IShoppingCartCommandService cartService, ILogger<RemoveProductFromCartCommandHandler> logger)
    {
        _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the command to remove a product from the shopping cart.
    /// </summary>
    /// <param name="request">The command containing the cart ID and product ID to remove.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result indicating whether the operation was successful.</returns>
    public async Task<Result<bool>> Handle(RemoveProductFromCartCommand request, CancellationToken cancellationToken)
    {
        if (request.CartId == Guid.Empty)
        {
            _logger.LogWarning("RemoveProductFromCartCommand failed: Cart ID is empty.");
            return Result.Failure<bool>("Cart ID cannot be empty.");
        }

        if (string.IsNullOrEmpty(request.ProductId))
        {
            _logger.LogWarning("RemoveProductFromCartCommand failed: Product ID is empty.");
            return Result.Failure<bool>("Product ID cannot be empty.");
        }

        _logger.LogInformation($"Attempting to remove product with ID: {request.ProductId} from cart with ID: {request.CartId}");

        var success = await _cartService.RemoveProductFromCartAsync(request.CartId, request.ProductId, cancellationToken);

        if (!success)
        {
            _logger.LogError($"Failed to remove product with ID: {request.ProductId} from cart with ID: {request.CartId}");
            return Result.Failure<bool>("Failed to remove product from the shopping cart.");
        }

        _logger.LogInformation($"Successfully removed product with ID: {request.ProductId} from cart with ID: {request.CartId}");
        return Result.Success(true);
    }
}