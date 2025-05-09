using MediatR;
using MerchStore.Application.ShoppingCart.Interfaces;
using MerchStore.Domain.ShoppingCart.Interfaces;
using MerchStore.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace MerchStore.Application.ShoppingCart.Queries;

/// <summary>
/// Handles the query to calculate the total price of a shopping cart.
/// </summary>
public class CalculateCartTotalQueryHandler : IRequestHandler<CalculateCartTotalQuery, Money>
{
    private readonly IShoppingCartQueryRepository _repository;
    private readonly ILogger<CalculateCartTotalQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CalculateCartTotalQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">The shopping cart repository.</param>
    /// <param name="logger">The logger for logging operations.</param>
    public CalculateCartTotalQueryHandler(IShoppingCartQueryRepository repository, ILogger<CalculateCartTotalQueryHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the query to calculate the total price of a shopping cart.
    /// </summary>
    /// <param name="request">The query containing the cart ID.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The total price of the shopping cart as a <see cref="Money"/> object.</returns>
    public async Task<Money> Handle(CalculateCartTotalQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Calculating total for cart with ID: {request.CartId}");

        var cart = await _repository.GetCartByIdAsync(request.CartId, cancellationToken);
        if (cart == null)
        {
            _logger.LogWarning($"Cart with ID {request.CartId} not found.");
            throw new InvalidOperationException($"Cart with ID {request.CartId} not found.");
        }

        var total = cart.CalculateTotal();
        _logger.LogInformation($"Successfully calculated total for cart with ID {request.CartId}: {total.Amount} {total.Currency}");
        return total;
    }
}