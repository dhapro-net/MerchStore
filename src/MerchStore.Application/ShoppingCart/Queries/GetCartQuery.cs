using MediatR;
using MerchStore.Application.ShoppingCart.DTOs;

namespace MerchStore.Application.ShoppingCart.Queries;

/// <summary>
/// Query to retrieve a shopping cart.
/// </summary>
public class GetCartQuery : IRequest<CartDto>
{
    public Guid CartId { get; }

    public GetCartQuery(Guid cartId)
    {
        CartId = cartId;
    }
}