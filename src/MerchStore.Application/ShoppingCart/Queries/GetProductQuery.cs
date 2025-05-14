using MediatR;
using MerchStore.Application.ShoppingCart.DTOs;
using MerchStore.Domain.ShoppingCart;

namespace MerchStore.Application.ShoppingCart.Queries;

public class GetProductQuery : IRequest<CartProductDto?>
{
    public Cart Cart { get; }
    public string ProductId { get; }

    public GetProductQuery(Cart cart, string productId)
    {
        Cart = cart ?? throw new ArgumentNullException(nameof(cart));
        ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
    }
}