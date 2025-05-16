using MediatR;
using MerchStore.Domain.ShoppingCart;

namespace MerchStore.Application.ShoppingCart.Commands;

/// <summary>
/// Command to add a shopping cart.
/// </summary>
public class AddCartCommand : IRequest
{
    public Cart Cart { get; }
    public CancellationToken CancellationToken { get; }

    public AddCartCommand(Cart cart, CancellationToken cancellationToken)
    {
        Cart = cart ?? throw new ArgumentNullException(nameof(cart));
        CancellationToken = cancellationToken;
    }
}