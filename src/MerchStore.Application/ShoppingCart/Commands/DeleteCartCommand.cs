using MediatR;

namespace MerchStore.Application.ShoppingCart.Commands;

/// <summary>
/// Command to delete a shopping cart.
/// </summary>
public class DeleteCartCommand : IRequest
{
    public Guid CartId { get; }

    public DeleteCartCommand(Guid cartId)
    {
        CartId = cartId;
    }
}