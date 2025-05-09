using MediatR;
using MerchStore.Application.ShoppingCart.Commands;

namespace MerchStore.Application.ShoppingCart.Handlers;

/// <summary>
/// Handles the UpdateCartCommand.
/// </summary>
public class UpdateCartCommandHandler : IRequestHandler<UpdateCartCommand>
{
    public Task<Unit> Handle(UpdateCartCommand request, CancellationToken cancellationToken)
    {
        if (request.Cart == null)
        {
            throw new ArgumentNullException(nameof(request.Cart), "Cart cannot be null.");
        }

        // Perform any additional business logic here if needed
        // For example, validate the cart or log the update operation

        return Task.FromResult(Unit.Value);
    }
}