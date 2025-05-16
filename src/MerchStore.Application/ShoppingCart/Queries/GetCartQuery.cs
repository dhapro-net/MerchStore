using MediatR;
using MerchStore.Application.ShoppingCart.DTOs;
using MerchStore.Domain.ShoppingCart;

namespace MerchStore.Application.ShoppingCart.Queries;

public class GetCartQuery : IRequest<CartDto>
{
    public Cart? Cart { get; }

    public Guid? CartId => Cart?.CartId; // Expose CartId from the Cart object, or null if Cart is null

    public GetCartQuery(Cart? cart)
    {
        Cart = cart;
    }
}