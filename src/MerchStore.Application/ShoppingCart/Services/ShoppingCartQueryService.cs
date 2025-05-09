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
        _logger.LogInformation("Retrieving or creating cart with ID: {CartId}.", cartId);

        // Attempt to retrieve the cart using Mediatr
        var cart = await _mediator.Send(new GetCartQuery(cartId), cancellationToken);
        if (cart == null)
        {
            _logger.LogWarning("Cart with ID {CartId} not found. Creating a new cart.", cartId);

            // Create a new cart
            cart = Cart.Create(cartId, _logger as ILogger<Cart>);

            // Add the new cart using Mediatr
            await _mediator.Send(new AddCartCommand(cart, cancellationToken));
        }

        // Map the cart to a CartDto
        return MapToCartDto(cart);
    }

    /// <summary>
    /// Retrieves or creates a shopping cart.
    /// </summary>
    public async Task<CartDto> GetCartAsync(GetCartQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching cart with ID: {CartId}.", query.CartId);

        var cart = await _mediator.Send(query, cancellationToken);
        if (cart == null)
        {
            _logger.LogWarning("Cart with ID {CartId} not found.", query.CartId);
            return CreateEmptyCartDto(query.CartId);
        }

        return MapToCartDto(cart);
    }

    public async Task<Money> CalculateCartTotalAsync(Guid cartId)
    {
        _logger.LogInformation("Calculating total for cart with ID: {CartId}.", cartId);

        // Retrieve the cart using Mediatr
        var cart = await _mediator.Send(new GetCartQuery(cartId));
        if (cart == null)
        {
            _logger.LogWarning("Cart with ID {CartId} not found. Returning default total.", cartId);
            return new Money(0, "SEK");
        }

        // Map CartDto.Products to CartProduct domain model
        var products = cart.Products.Select(dto => new CartProduct(
            dto.ProductId,
            dto.ProductName,
            dto.UnitPrice,
            dto.Quantity
        )).ToList();

        // Use CartCalculationService to calculate the total
        return _cartCalculationService.CalculateTotal(products);
    }

    /// <summary>
    /// Retrieves the shopping cart details.
    /// </summary>
    public async Task<CartDto> GetCartAsync(Guid cartId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching cart with ID: {CartId}.", cartId);

        var cart = await _mediator.Send(new GetCartQuery(cartId), cancellationToken);
        if (cart == null)
        {
            _logger.LogWarning("Cart with ID {CartId} not found.", cartId);
            return CreateEmptyCartDto(cartId);
        }

        return MapToCartDto(cart);
    }

    /// <summary>
    /// Retrieves a summary of the shopping cart.
    /// </summary>
    /// <summary>
    /// Retrieves a summary of the shopping cart.
    /// </summary>
    public async Task<CartSummaryDto> GetCartSummaryAsync(GetCartSummaryQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching cart summary with ID: {CartId}.", query.CartId);

        var cart = await _mediator.Send(query, cancellationToken);
        if (cart == null)
        {
            _logger.LogWarning("Cart summary with ID {CartId} not found.", query.CartId);
            return new CartSummaryDto
            {
                CartId = query.CartId,
                ProductCount = 0,
                TotalPrice = new Money(0, "SEK") // Default total price
            };
        }

        // Use CalculateCartTotalAsync to calculate the total price
        var totalPrice = await CalculateCartTotalAsync(cart.CartId);

        // Use ProductCountAsync to get the total number of products
        var productCount = await ProductCountAsync(cart.CartId, cancellationToken);

        return new CartSummaryDto
        {
            CartId = cart.CartId,
            ProductCount = productCount,
            TotalPrice = totalPrice
        };
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

        var cart = await _mediator.Send(new GetCartQuery(cartId), cancellationToken);
        var product = cart?.Products.FirstOrDefault(i => i.ProductId == productId);
        if (product == null)
        {
            _logger.LogWarning("Product {ProductId} not found in cart with ID {CartId}.", productId, cartId);
            return null;
        }

        return new CartProductDto
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            UnitPrice = product.UnitPrice,
            Quantity = product.Quantity
        };
    }

    /// <summary>
    /// Maps a shopping cart to a CartDto.
    /// </summary>
    private CartDto MapToCartDto(Cart cart)
    {
        return new CartDto
        {
            CartId = cart.CartId,
            TotalPrice = cart.CalculateTotal(),
            TotalProducts = cart.Products.Sum(i => i.Quantity),
            LastUpdated = cart.LastUpdated,
            Products = cart.Products.Select(product => new CartProductDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                UnitPrice = product.UnitPrice,
                Quantity = product.Quantity
            }).ToList()
        };
    }

    /// <summary>
    /// Creates an empty CartDto.
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