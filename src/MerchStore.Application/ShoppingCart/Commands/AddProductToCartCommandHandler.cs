using MediatR;
using MerchStore.Application.Common;
using MerchStore.Application.ShoppingCart.Interfaces;
using Microsoft.Extensions.Logging;

namespace MerchStore.Application.ShoppingCart.Commands;

/// <summary>
/// Handles the command to add a product to the shopping cart.
/// </summary>
public class AddProductToCartCommandHandler : IRequestHandler<AddProductToCartCommand, Result<bool>>
{
    private readonly IShoppingCartCommandService _cartService;
    private readonly ILogger<AddProductToCartCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddProductToCartCommandHandler"/> class.
    /// </summary>
    /// <param name="cartService">The shopping cart service.</param>
    /// <param name="logger">The logger for logging operations.</param>
    public AddProductToCartCommandHandler(IShoppingCartCommandService cartService, ILogger<AddProductToCartCommandHandler> logger)
    {
        _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the command to add a product to the shopping cart.
    /// </summary>
    /// <param name="request">The command containing the cart ID, product ID, and quantity.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result indicating whether the operation was successful.</returns>
    public async Task<Result<bool>> Handle(AddProductToCartCommand request, CancellationToken cancellationToken)
    {
        if (request.CartId == Guid.Empty)
        {
            _logger.LogWarning("AddProductToCartCommand failed: Cart ID is empty.");
            return Result.Failure<bool>("Cart ID cannot be empty");
        }

        if (string.IsNullOrEmpty(request.ProductId))
        {
            _logger.LogWarning("AddProductToCartCommand failed: Product ID is empty.");
            return Result.Failure<bool>("Product ID cannot be empty");
        }

        if (request.Quantity <= 0)
        {
            _logger.LogWarning("AddProductToCartCommand failed: Quantity is less than or equal to zero.");
            return Result.Failure<bool>("Quantity must be greater than zero");
        }

        var success = await _cartService.AddProductToCartAsync(
            request.CartId,
            request.ProductId,
            request.Quantity,
            cancellationToken);

        if (!success)
        {
            _logger.LogError("AddProductToCartCommand failed: Unable to add product to cart. Product may not exist or is unavailable.");
            return Result.Failure<bool>("Failed to add product to cart. The product may not exist or is unavailable.");
        }

        _logger.LogInformation("AddProductToCartCommand succeeded: Product added to cart successfully.");
        return Result.Success(true);
    }
}