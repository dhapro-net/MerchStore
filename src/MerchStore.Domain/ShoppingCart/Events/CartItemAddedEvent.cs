using MerchStore.Domain.Common;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.Domain.ShoppingCart.Events;

public class CartItemAddedEvent : DomainEvent
{
    public Guid CartId { get; }
    public string ProductId { get; }
    public string ProductName { get; }
    public Money UnitPrice { get; }
    public int Quantity { get; }

    public CartItemAddedEvent(Guid cartId, string productId, string productName, Money unitPrice, int quantity)
    {
        CartId = cartId;
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }
}