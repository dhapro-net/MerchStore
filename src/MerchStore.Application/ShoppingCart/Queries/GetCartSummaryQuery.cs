using MediatR;
using MerchStore.Application.ShoppingCart.DTOs;
using MerchStore.Domain.ShoppingCart;

namespace MerchStore.Application.ShoppingCart.Queries;

public class GetCartSummaryQuery : IRequest<CartSummaryDto>
{
    public Guid CartId { get; }
    public Cart? Cart { get; }

    public GetCartSummaryQuery(Guid cartId, Cart? cart)
    {
        CartId = cartId;
        Cart = cart;
    }
}