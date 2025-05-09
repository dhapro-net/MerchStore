using MediatR;
using MerchStore.Application.ShoppingCart.Commands;
using MerchStore.Application.ShoppingCart.Interfaces;
using MerchStore.Domain.ShoppingCart;
using Microsoft.Extensions.Logging;

namespace MerchStore.Application.ShoppingCart.Services;

/// <summary>
/// Handles command operations for managing the shopping cart.
/// </summary>
public class ShoppingCartCommandService : IShoppingCartCommandService
{
    private readonly IMediator _mediator;
    private readonly ILogger<ShoppingCartCommandService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShoppingCartCommandService"/> class.
    /// </summary>
    /// <param name="mediator">The Mediatr instance for handling commands.</param>
    /// <param name="logger">The logger for logging operations.</param>
    public ShoppingCartCommandService(IMediator mediator, ILogger<ShoppingCartCommandService> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    public async Task AddAsync(Cart cart, CancellationToken cancellationToken)
{
    if (cart == null)
        throw new ArgumentNullException(nameof(cart));

    _logger.LogInformation("Adding a new cart with ID: {CartId}.", cart.CartId);

    await _mediator.Send(new AddCartCommand(cart, cancellationToken));

    _logger.LogInformation("Successfully added a new cart with ID: {CartId}.", cart.CartId);
}

    /// <summary>
    /// Adds a product to the shopping cart.
    /// </summary>
    public async Task<bool> AddProductToCartAsync(Guid cartId, string productId, int quantity, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding ProductId: {ProductId} with Quantity: {Quantity} to CartId: {CartId}.", productId, quantity, cartId);

        var result = await _mediator.Send(new AddProductToCartCommand(cartId, productId, quantity, cancellationToken));
        if (!result.IsSuccess)
        {
            _logger.LogError("Failed to add ProductId: {ProductId} to CartId: {CartId}. Error: {Error}", productId, cartId, result.Error);
            return false;
        }

        _logger.LogInformation("Successfully added ProductId: {ProductId} to CartId: {CartId}.", productId, cartId);
        return result.Value;
    }

    /// <summary>
    /// Removes a product from the shopping cart.
    /// </summary>
    public async Task<bool> RemoveProductFromCartAsync(Guid cartId, string productId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing ProductId: {ProductId} from CartId: {CartId}.", productId, cartId);

        var result = await _mediator.Send(new RemoveProductFromCartCommand(cartId, productId, cancellationToken));
        if (!result.IsSuccess)
        {
            _logger.LogError("Failed to remove ProductId: {ProductId} from CartId: {CartId}. Error: {Error}", productId, cartId, result.Error);
            return false;
        }

        _logger.LogInformation("Successfully removed ProductId: {ProductId} from CartId: {CartId}.", productId, cartId);
        return result.Value;
    }

    /// <summary>
    /// Updates the quantity of a product in the shopping cart.
    /// </summary>
    public async Task<bool> UpdateProductQuantityAsync(Guid cartId, string productId, int quantity)
    {
        _logger.LogInformation("Updating quantity for ProductId: {ProductId} to {Quantity} in CartId: {CartId}.", productId, quantity, cartId);

        var result = await _mediator.Send(new UpdateCartProductQuantityCommand(cartId, productId, quantity));
        if (!result.IsSuccess)
        {
            _logger.LogError("Failed to update quantity for ProductId: {ProductId} in CartId: {CartId}. Error: {Error}", productId, cartId, result.Error);
            return false;
        }

        _logger.LogInformation("Successfully updated quantity for ProductId: {ProductId} in CartId: {CartId}.", productId, cartId);
        return result.Value;
    }

    /// <summary>
    /// Clears all products from the shopping cart.
    /// </summary>
    public async Task<bool> ClearCartAsync(Guid cartId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Clearing all products from CartId: {CartId}.", cartId);

        var result = await _mediator.Send(new ClearCartCommand(cartId));
        if (!result.IsSuccess)
        {
            _logger.LogError("Failed to clear CartId: {CartId}. Error: {Error}", cartId, result.Error);
            return false;
        }

        _logger.LogInformation("Successfully cleared CartId: {CartId}.", cartId);
        return result.Value;
    }
}