using MediatR;
using MerchStore.Application.Common;
using MerchStore.Application.ShoppingCart.Interfaces;
using Microsoft.Extensions.Logging;

namespace MerchStore.Application.ShoppingCart.Commands;

/// <summary>
/// Handles the command to clear the shopping cart.
/// </summary>
public class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, Result<bool>>
{
    private readonly IShoppingCartCommandService _cartService;
    private readonly ILogger<ClearCartCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClearCartCommandHandler"/> class.
    /// </summary>
    /// <param name="cartService">The shopping cart service.</param>
    /// <param name="logger">The logger for logging operations.</param>
    public ClearCartCommandHandler(IShoppingCartCommandService cartService, ILogger<ClearCartCommandHandler> logger)
    {
        _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the command to clear the shopping cart.
    /// </summary>
    /// <param name="request">The command containing the cart ID to clear.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result indicating whether the operation was successful.</returns>
    public async Task<Result<bool>> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        if (request.CartId == Guid.Empty)
        {
            _logger.LogWarning("ClearCartCommand failed: Cart ID is empty.");
            return Result.Failure<bool>("Cart ID cannot be empty.");
        }

        _logger.LogInformation($"Attempting to clear the shopping cart with ID: {request.CartId}");

        var success = await _cartService.ClearCartAsync(request.CartId, cancellationToken);

        if (!success)
        {
            _logger.LogError($"Failed to clear the shopping cart with ID: {request.CartId}");
            return Result.Failure<bool>("Failed to clear the shopping cart.");
        }

        _logger.LogInformation($"Successfully cleared the shopping cart with ID: {request.CartId}");
        return Result.Success(true);
    }
}