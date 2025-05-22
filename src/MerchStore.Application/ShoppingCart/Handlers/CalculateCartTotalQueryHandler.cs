using MediatR;
using MerchStore.Application.ShoppingCart.Queries;
using MerchStore.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
namespace MerchStore.Application.ShoppingCart.Handlers
{
public class CalculateCartTotalQueryHandler : IRequestHandler<CalculateCartTotalQuery, Money>
{
    private readonly ILogger<CalculateCartTotalQueryHandler> _logger;

    public CalculateCartTotalQueryHandler(ILogger<CalculateCartTotalQueryHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<Money> Handle(CalculateCartTotalQuery request, CancellationToken cancellationToken)
    {
       

        _logger.LogInformation("Calculating total for cart with ID: {CartId}.", request.Cart.CartId);

        // Calculate the total price of the cart
        var total = request.Cart.CalculateTotal();
        _logger.LogInformation("Successfully calculated total for cart with ID {CartId}: {TotalAmount} {Currency}.", request.Cart.CartId, total.Amount, total.Currency);

        return Task.FromResult(total);
    }
}
}