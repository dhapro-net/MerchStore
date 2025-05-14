using MediatR;
using MerchStore.Domain.ShoppingCart;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.Application.ShoppingCart.Queries;

public class CalculateCartTotalQuery : IRequest<Money>
{
    public Cart Cart { get; }

    public CalculateCartTotalQuery(Cart cart)
    {
        Cart = cart ?? throw new ArgumentNullException(nameof(cart));
    }
}