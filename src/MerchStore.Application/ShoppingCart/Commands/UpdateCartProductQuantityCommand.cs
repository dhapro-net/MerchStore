using System;
using MediatR;
using MerchStore.Application.Common;
using MerchStore.Domain.ShoppingCart;

namespace MerchStore.Application.ShoppingCart.Commands;

public class UpdateCartProductQuantityCommand : IRequest<Result<bool>>
{
    public Cart Cart { get; }
    public string ProductId { get; }
    public int Quantity { get; }

    public UpdateCartProductQuantityCommand(Cart cart, string productId, int quantity)
    {
        Cart = cart ?? throw new ArgumentNullException(nameof(cart));
        ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
        Quantity = quantity > 0 ? quantity : throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
    }
}