using MediatR;
using MerchStore.Application.ShoppingCart.Commands;
using MerchStore.Application.ShoppingCart.DTOs;
using MerchStore.Application.ShoppingCart.Queries;
using MerchStore.Application.ShoppingCart.Interfaces;
using MerchStore.Domain.ShoppingCart;
using MerchStore.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using MerchStore.Domain.Services;

namespace MerchStore.Application.ShoppingCart.Services;

/// <summary>
/// Handles query operations for retrieving shopping cart data.
/// </summary>
public class ShoppingCartQueryService : IShoppingCartQueryService
{
    private readonly IMediator _mediator;
    private readonly ILogger<ShoppingCartQueryService> _logger;
    private readonly CartCalculationService _cartCalculationService;


    /// <summary>
    /// Initializes a new instance of the <see cref="ShoppingCartQueryService"/> class.
    /// </summary>
    /// <param name="mediator">The Mediatr instance for sending commands and queries.</param>
    /// <param name="logger">The logger for logging operations.</param>
    public ShoppingCartQueryService(IMediator mediator, ILogger<ShoppingCartQueryService> logger, CartCalculationService cartCalculationService)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cartCalculationService = cartCalculationService;

    }
    /// <summary>
    /// Retrieves an existing shopping cart or creates a new one if it doesn't exist.
    /// </summary>
    /// <param name="cartId">The ID of the shopping cart.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The shopping cart as a <see cref="CartDto"/>.</returns>
 public async Task<CartDto> GetOrCreateCartAsync(Guid cartId, CancellationToken cancellationToken)
{
    _logger.LogInformation("Retrieving cart with ID: {CartId}.", cartId);

    // Check if the cartId is Guid.Empty
    if (cartId == Guid.Empty)
    {
        _logger.LogWarning("Cart ID is Guid.Empty. Creating a new cart.");
        return await CreateNewCartAsync(cancellationToken);
    }

    // Query: Retrieve the cart using GetCartQuery
    var cart = await _mediator.Send(new GetCartQuery(cartId), cancellationToken);
    if (cart == null || cart.CartId == Guid.Empty)
    {
        _logger.LogWarning("Cart with ID {CartId} not found or invalid. Creating a new cart.", cartId);
        return await CreateNewCartAsync(cancellationToken);
    }

    return cart;
}

private async Task<CartDto> CreateNewCartAsync(CancellationToken cancellationToken)
{
    var newCartId = Guid.NewGuid();
    _logger.LogInformation("Creating a new cart with ID: {CartId}.", newCartId);

    // Command: Create a new cart using Mediatr
    return await _mediator.Send(new CreateCartCommand(newCartId), cancellationToken);
}





    /// <summary>
    /// Retrieves or creates a shopping cart.
    /// </summary>
    public async Task<CartDto> GetCartAsync(GetCartQuery query, CancellationToken cancellationToken)
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        _logger.LogInformation("Fetching cart with ID: {CartId}.", query.CartId);

        var cart = await _mediator.Send(query, cancellationToken);
        if (cart == null)
        {
            _logger.LogWarning("Cart with ID {CartId} not found.", query.CartId);
            return CreateEmptyCartDto(query.CartId);
        }

        return cart;
    }

    public async Task<Money> CalculateCartTotalAsync(Guid cartId)
    {
        _logger.LogInformation("Calculating total for cart with ID: {CartId}.", cartId);

        // Use Mediatr to send the CalculateCartTotalQuery
        var total = await _mediator.Send(new CalculateCartTotalQuery(cartId));
        return total;
    }


    /// <summary>
    /// Retrieves a summary of the shopping cart.
    /// </summary>
    public async Task<CartSummaryDto> GetCartSummaryAsync(GetCartSummaryQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching cart summary with ID: {CartId}.", query.CartId);

        // Use Mediatr to send the GetCartSummaryQuery
        return await _mediator.Send(query, cancellationToken);
    }

    /// <summary>
    /// Checks if the cart has any products.
    /// </summary>
    public async Task<bool> HasProductsAsync(Guid cartId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Checking if cart with ID {CartId} has products.", cartId);

        var cart = await _mediator.Send(new GetCartQuery(cartId), cancellationToken);
        return cart?.Products.Any() ?? false;
    }

    /// <summary>
    /// Gets the total number of products in the cart.
    /// </summary>
    public async Task<int> ProductCountAsync(Guid cartId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting product count for cart with ID {CartId}.", cartId);

        var cart = await _mediator.Send(new GetCartQuery(cartId), cancellationToken);
        return cart?.Products.Sum(i => i.Quantity) ?? 0;
    }

    /// <summary>
    /// Checks if the cart contains a specific product.
    /// </summary>
    public async Task<bool> ContainsProductAsync(Guid cartId, string productId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(productId))
            throw new ArgumentException("Product ID cannot be null or empty", nameof(productId));

        _logger.LogInformation("Checking if cart with ID {CartId} contains product {ProductId}.", cartId, productId);

        var cart = await _mediator.Send(new GetCartQuery(cartId), cancellationToken);
        return cart?.Products.Any(i => i.ProductId == productId) ?? false;
    }

    /// <summary>
    /// Retrieves a specific product from the cart.
    /// </summary>
    public async Task<CartProductDto?> GetProductAsync(Guid cartId, string productId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(productId))
            throw new ArgumentException("Product ID cannot be null or empty", nameof(productId));

        _logger.LogInformation("Fetching product {ProductId} from cart with ID {CartId}.", productId, cartId);

        // Use Mediatr to send the GetProductQuery
        return await _mediator.Send(new GetProductQuery(cartId, productId), cancellationToken);
    }

 

    /// <summary>
    /// Creates an empty CartDto.
    /// 
    ///Should be a mapper class I think.
    /// </summary>
    private CartDto CreateEmptyCartDto(Guid cartId)
    {
        return new CartDto
        {
            CartId = cartId,
            Products = new List<CartProductDto>(),
            TotalPrice = new Money(0, "SEK"),
            TotalProducts = 0,
            LastUpdated = DateTime.UtcNow
        };
    }
}