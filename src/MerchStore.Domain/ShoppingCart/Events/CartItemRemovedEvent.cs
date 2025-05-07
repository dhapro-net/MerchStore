using MerchStore.Domain.Common;

namespace MerchStore.Domain.ShoppingCart.Events;

public class CartItemRemovedEvent : DomainEvent
{
    public Guid CartId { get; }
    public string ProductId { get; }

    public CartItemRemovedEvent(Guid cartId, string productId)
    {
        CartId = cartId;
        ProductId = productId;
    }
}