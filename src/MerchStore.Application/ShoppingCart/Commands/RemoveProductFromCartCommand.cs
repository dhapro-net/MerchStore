using MediatR;
using MerchStore.Application.Common;
using MerchStore.Domain.ShoppingCart;

namespace MerchStore.Application.ShoppingCart.Commands;

public class RemoveProductFromCartCommand : IRequest<Result<bool>>
{
    public Cart Cart { get; }
    public string ProductId { get; }

    public RemoveProductFromCartCommand(Cart cart, string productId)
    {
        Cart = cart ?? throw new ArgumentNullException(nameof(cart));
        ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
    }
}