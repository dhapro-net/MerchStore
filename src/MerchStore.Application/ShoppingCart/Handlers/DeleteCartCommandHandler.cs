using MediatR;
using MerchStore.Application.ShoppingCart.Commands;
using MerchStore.Application.ShoppingCart.Interfaces;
using Microsoft.Extensions.Logging;

namespace MerchStore.Application.ShoppingCart.Handlers;

/// <summary>
/// Handles the DeleteCartCommand.
/// </summary>
public class DeleteCartCommandHandler : IRequestHandler<DeleteCartCommand>
{
    private readonly ILogger<DeleteCartCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteCartCommandHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger for logging operations.</param>
    public DeleteCartCommandHandler(ILogger<DeleteCartCommandHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the DeleteCartCommand.
    /// </summary>
    /// <param name="request">The command containing the cart ID to delete.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A unit result indicating the operation was successful.</returns>
    public Task<Unit> Handle(DeleteCartCommand request, CancellationToken cancellationToken)
    {
        if (request.CartId == Guid.Empty)
        {
            _logger.LogWarning("DeleteCartCommand failed: Cart ID is empty.");
            throw new ArgumentException("Cart ID cannot be empty.", nameof(request.CartId));
        }

        try
        {
            _logger.LogInformation("Processing DeleteCartCommand for cart with ID {CartId}.", request.CartId);

            // Perform any additional business logic here if needed

            _logger.LogInformation("Successfully processed DeleteCartCommand for cart with ID {CartId}.", request.CartId);
            return Task.FromResult(Unit.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing DeleteCartCommand for cart with ID {CartId}.", request.CartId);
            throw;
        }
    }
}