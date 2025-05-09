using MediatR;
using MerchStore.Domain.ShoppingCart;

namespace MerchStore.Application.ShoppingCart.Queries;

/// <summary>
/// Query to retrieve a shopping cart.
/// </summary>
public class GetCartQuery : IRequest<Cart>
{
    public Guid CartId { get; }

    public GetCartQuery(Guid cartId)
    {
        CartId = cartId;
    }
}