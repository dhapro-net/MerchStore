using MediatR;
using MerchStore.Domain.ShoppingCart;

namespace MerchStore.Application.ShoppingCart.Commands;

/// <summary>
/// Command to update a shopping cart.
/// </summary>
public class UpdateCartCommand : IRequest
{
    public Cart Cart { get; }

    public UpdateCartCommand(Cart cart)
    {
        Cart = cart ?? throw new ArgumentNullException(nameof(cart));
    }
}