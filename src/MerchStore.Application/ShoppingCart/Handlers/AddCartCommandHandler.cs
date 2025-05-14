using MediatR;
using MerchStore.Application.ShoppingCart.Commands;
using Microsoft.Extensions.Logging;

namespace MerchStore.Application.ShoppingCart.Handlers;

/// <summary>
/// Handles the AddCartCommand.
/// </summary>
public class AddCartCommandHandler : IRequestHandler<AddCartCommand>
{
    private readonly ILogger<AddCartCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddCartCommandHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger for logging operations.</param>
    public AddCartCommandHandler(ILogger<AddCartCommandHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the AddCartCommand.
    /// </summary>
    /// <param name="request">The command containing the cart to add.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A unit result indicating the operation was successful.</returns>
    public Task<Unit> Handle(AddCartCommand request, CancellationToken cancellationToken)
    {
        if (request.Cart == null)
        {
            _logger.LogWarning("AddCartCommand failed: Cart is null.");
            throw new ArgumentNullException(nameof(request.Cart));
        }

        try
        {
            _logger.LogInformation("Processing AddCartCommand for cart with ID {CartId}.", request.Cart.CartId);

            // Perform any additional business logic here if needed

            _logger.LogInformation("Successfully processed AddCartCommand for cart with ID {CartId}.", request.Cart.CartId);
            return Task.FromResult(Unit.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing AddCartCommand for cart with ID {CartId}.", request.Cart.CartId);
            throw;
        }
    }
}