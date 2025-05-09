using MediatR;
using MerchStore.Application.ShoppingCart.DTOs;
using MerchStore.Application.ShoppingCart.Interfaces;
using MerchStore.Application.ShoppingCart.Mappers;
using MerchStore.Application.ShoppingCart.Queries;
using MerchStore.Domain.ShoppingCart;
using MerchStore.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace MerchStore.Application.ShoppingCart.Services;

/// <summary>
/// Handles query operations for retrieving shopping cart data.
/// </summary>
public class ShoppingCartQueryService : IShoppingCartQueryService
{
    private readonly IMediator _mediator;
    private readonly ILogger<ShoppingCartQueryService> _logger;
    private readonly ILogger<Cart> _cartLogger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShoppingCartQueryService"/> class.
    /// </summary>
    /// <param name="mediator">The Mediatr instance for sending commands and queries.</param>
    /// <param name="logger">The logger for logging operations.</param>
    /// <param name="cartLogger">The logger for logging cart-specific operations.</param>
    public ShoppingCartQueryService(IMediator mediator, ILogger<ShoppingCartQueryService> logger, ILogger<Cart> cartLogger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cartLogger = cartLogger ?? throw new ArgumentNullException(nameof(cartLogger));
    }

    /// <summary>
    /// Retrieves or creates a shopping cart.
    /// </summary>
    public async Task<CartDto> GetCartAsync(GetCartQuery query, CancellationToken cancellationToken)
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        _logger.LogInformation("Fetching cart with ID: {CartId}.", query.Cart.CartId);

        var cart = await _mediator.Send(query, cancellationToken);
        if (cart == null)
        {
            _logger.LogWarning("Cart with ID {CartId} not found.", query.Cart.CartId);
            return CartMapper.CreateEmptyCartDto(query.Cart.CartId);
        }

        return cart;
    }

    /// <summary>
    /// Calculates the total price of the shopping cart.
    /// </summary>
    public async Task<Money> CalculateCartTotalAsync(Guid cartId)
    {
        _logger.LogInformation("Calculating total for cart with ID: {CartId}.", cartId);

        // Create a Cart object using the factory method
        var cart = Cart.Create(cartId, _cartLogger);

        // Pass the Cart object to the GetCartQuery
        var query = new GetCartQuery(cart);

        // Use Mediatr to send the CalculateCartTotalQuery
        return await _mediator.Send(query);
    }

    /// <summary>
    /// Retrieves a summary of the shopping cart.
    /// </summary>
    public async Task<CartSummaryDto> GetCartSummaryAsync(GetCartSummaryQuery query, CancellationToken cancellationToken)
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        _logger.LogInformation("Fetching cart summary with ID: {CartId}.", query.Cart.CartId);

        // Use Mediatr to send the GetCartSummaryQuery
        return await _mediator.Send(query, cancellationToken);
    }

    /// <summary>
    /// Checks if the cart has any products.
    /// </summary>
    public async Task<bool> HasProductsAsync(Guid cartId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Checking if cart with ID {CartId} has products.", cartId);

        // Create a Cart object using the factory method
        var cart = Cart.Create(cartId, _cartLogger);

        // Pass the Cart object to the GetCartQuery
        var query = new GetCartQuery(cart);

        var cartDto = await _mediator.Send(query, cancellationToken);
        return cartDto?.Products.Any() ?? false;
    }

    /// <summary>
    /// Gets the total number of products in the cart.
    /// </summary>
    public async Task<int> ProductCountAsync(Guid cartId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting product count for cart with ID {CartId}.", cartId);

        // Create a Cart object using the factory method
        var cart = Cart.Create(cartId, _cartLogger);

        // Pass the Cart object to the GetCartQuery
        var query = new GetCartQuery(cart);

        var cartDto = await _mediator.Send(query, cancellationToken);
        return cartDto?.Products.Sum(i => i.Quantity) ?? 0;
    }

    /// <summary>
    /// Checks if the cart contains a specific product.
    /// </summary>
    public async Task<bool> ContainsProductAsync(Guid cartId, string productId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(productId))
            throw new ArgumentException("Product ID cannot be null or empty", nameof(productId));

        _logger.LogInformation("Checking if cart with ID {CartId} contains product {ProductId}.", cartId, productId);

        // Create a Cart object using the factory method
        var cart = Cart.Create(cartId, _cartLogger);

        // Pass the Cart object to the GetCartQuery
        var query = new GetCartQuery(cart);

        var cartDto = await _mediator.Send(query, cancellationToken);
        return cartDto?.Products.Any(i => i.ProductId == productId) ?? false;
    }

    /// <summary>
    /// Retrieves a specific product from the cart.
    /// </summary>
    public async Task<CartProductDto?> GetProductAsync(Guid cartId, string productId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(productId))
            throw new ArgumentException("Product ID cannot be null or empty", nameof(productId));

        _logger.LogInformation("Fetching product {ProductId} from cart with ID {CartId}.", productId, cartId);

        // Create a Cart object using the factory method
        var cart = Cart.Create(cartId, _cartLogger);

        // Pass the Cart object to the GetCartQuery
        var query = new GetCartQuery(cart);

        // Use Mediatr to send the GetProductQuery
        return await _mediator.Send(query, cancellationToken);
    }
}