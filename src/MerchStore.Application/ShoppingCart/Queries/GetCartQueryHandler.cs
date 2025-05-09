using MediatR;
using MerchStore.Application.ShoppingCart.DTOs;
using MerchStore.Domain.ShoppingCart.Interfaces;
using MerchStore.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace MerchStore.Application.ShoppingCart.Queries;

/// <summary>
/// Handles the query to retrieve a shopping cart.
/// </summary>
public class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartDto>
{
    private readonly IShoppingCartQueryRepository _repository;
    private readonly ILogger<GetCartQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCartQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">The shopping cart repository.</param>
    /// <param name="logger">The logger for logging operations.</param>
    public GetCartQueryHandler(IShoppingCartQueryRepository repository, ILogger<GetCartQueryHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the query to retrieve a shopping cart.
    /// </summary>
    /// <param name="request">The query containing the cart ID.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The shopping cart details as a <see cref="CartDto"/>.</returns>
    public async Task<CartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Fetching cart with ID: {request.CartId}");

        var cart = await _repository.GetCartByIdAsync(request.CartId, cancellationToken);

        if (cart == null)
        {
            _logger.LogWarning($"Cart with ID {request.CartId} not found. Returning a new empty cart.");
            return new CartDto
            {
                CartId = request.CartId,
                Products = new List<CartProductDto>(),
                TotalPrice = new Money(0, "SEK"),
                TotalProducts = 0,
                LastUpdated = DateTime.UtcNow
            };
        }

        _logger.LogInformation($"Successfully retrieved cart with ID {request.CartId}");
        return new CartDto
        {
            CartId = cart.Id,
            Products = cart.Products.Select(product => new CartProductDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                UnitPrice = product.UnitPrice,
                Quantity = product.Quantity
            }).ToList(),
            TotalPrice = cart.CalculateTotal(),
            TotalProducts = cart.Products.Sum(i => i.Quantity),
            LastUpdated = cart.LastUpdated
        };
    }
}