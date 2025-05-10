using MediatR;
using MerchStore.Application.ShoppingCart.Commands;
using MerchStore.Application.ShoppingCart.Interfaces;
using MerchStore.Application.ShoppingCart.Queries;
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
    private readonly ILogger<Cart> _cartLogger;

    public ShoppingCartCommandService(IMediator mediator, ILogger<ShoppingCartCommandService> logger, ILogger<Cart> cartLogger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cartLogger = cartLogger ?? throw new ArgumentNullException(nameof(cartLogger));
    }

    /// <summary>
    /// Adds a new cart.
    /// </summary>
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
        if (cartId == Guid.Empty)
            throw new ArgumentException("Cart ID cannot be empty.", nameof(cartId));

        if (string.IsNullOrEmpty(productId))
            throw new ArgumentException("Product ID cannot be null or empty.", nameof(productId));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

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
        if (cartId == Guid.Empty)
            throw new ArgumentException("Cart ID cannot be empty.", nameof(cartId));

        if (string.IsNullOrEmpty(productId))
            throw new ArgumentException("Product ID cannot be null or empty.", nameof(productId));

        _logger.LogInformation("Removing ProductId: {ProductId} from CartId: {CartId}.", productId, cartId);

        var cart = Cart.Create(cartId, _cartLogger);
        var query = new GetCartQuery(cart);
        var cartDto = await _mediator.Send(query, cancellationToken);

        if (cartDto == null)
        {
            _logger.LogError("Failed to retrieve cart with ID: {CartId}.", cartId);
            return false;
        }

        var result = await _mediator.Send(new RemoveProductFromCartCommand(cart, productId), cancellationToken);
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
    public async Task<bool> UpdateProductQuantityAsync(Guid cartId, string productId, int quantity, CancellationToken cancellationToken)
    {
        if (cartId == Guid.Empty)
            throw new ArgumentException("Cart ID cannot be empty.", nameof(cartId));

        if (string.IsNullOrEmpty(productId))
            throw new ArgumentException("Product ID cannot be null or empty.", nameof(productId));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        _logger.LogInformation("Updating quantity for ProductId: {ProductId} to {Quantity} in CartId: {CartId}.", productId, quantity, cartId);

        var cart = Cart.Create(cartId, _cartLogger);
        var query = new GetCartQuery(cart);
        var cartDto = await _mediator.Send(query, cancellationToken);

        if (cartDto == null)
        {
            _logger.LogError("Failed to retrieve cart with ID: {CartId}.", cartId);
            return false;
        }

        var result = await _mediator.Send(new UpdateCartProductQuantityCommand(cart, productId, quantity), cancellationToken);
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
        if (cartId == Guid.Empty)
            throw new ArgumentException("Cart ID cannot be empty.", nameof(cartId));

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