using MediatR;
using MerchStore.Application.ShoppingCart.DTOs;
using MerchStore.Application.ShoppingCart.Interfaces;
using Microsoft.Extensions.Logging;

namespace MerchStore.Application.ShoppingCart.Queries;

/// <summary>
/// Handles the query to retrieve the summary of a shopping cart.
/// </summary>
public class GetCartSummaryQueryHandler : IRequestHandler<GetCartSummaryQuery, CartSummaryDto>
{
    private readonly IShoppingCartQueryService _queryService;
    private readonly ILogger<GetCartSummaryQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCartSummaryQueryHandler"/> class.
    /// </summary>
    /// <param name="queryService">The shopping cart query service.</param>
    /// <param name="logger">The logger for logging operations.</param>
    public GetCartSummaryQueryHandler(IShoppingCartQueryService queryService, ILogger<GetCartSummaryQueryHandler> logger)
    {
        _queryService = queryService ?? throw new ArgumentNullException(nameof(queryService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the query to retrieve the summary of a shopping cart.
    /// </summary>
    /// <param name="request">The query containing the cart ID.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The shopping cart summary as a <see cref="CartSummaryDto"/>.</returns>
    public async Task<CartSummaryDto> Handle(GetCartSummaryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Retrieving summary for cart with ID: {request.CartId}");

        var cartSummary = await _queryService.GetCartSummaryAsync(request.CartId, cancellationToken);

        _logger.LogInformation($"Successfully retrieved summary for cart with ID: {request.CartId}");
        return cartSummary;
    }
}