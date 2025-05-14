using MediatR;
using MerchStore.Application.Common;
using MerchStore.Application.ShoppingCart.Commands;
using MerchStore.Application.ShoppingCart.Interfaces;
using Microsoft.Extensions.Logging;
namespace MerchStore.Application.ShoppingCart.Handlers
{
public class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, Result<bool>>
{
    private readonly ILogger<ClearCartCommandHandler> _logger;

    public ClearCartCommandHandler(ILogger<ClearCartCommandHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<Result<bool>> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        if (request.CartId == Guid.Empty)
        {
            _logger.LogWarning("ClearCartCommand failed: Cart ID is empty.");
            return Task.FromResult(Result.Failure<bool>("Cart ID cannot be empty."));
        }

        try
        {
            _logger.LogInformation("Clearing the shopping cart with ID: {CartId}", request.CartId);

            // Business logic for clearing the cart (if any additional logic is needed)
            // For example, you might log the action or perform other non-HTTP-specific tasks.

            _logger.LogInformation("Successfully cleared the shopping cart with ID: {CartId}", request.CartId);
            return Task.FromResult(Result.Success(true));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while clearing the shopping cart with ID: {CartId}", request.CartId);
            return Task.FromResult(Result.Failure<bool>("An unexpected error occurred while clearing the shopping cart."));
        }
    }
}
}