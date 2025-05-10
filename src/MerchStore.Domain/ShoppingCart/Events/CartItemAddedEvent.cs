using System;
using MerchStore.Domain.Common;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.Domain.ShoppingCart.Events
{
    public class CartProductAddedEvent : DomainEvent
    {
        public Guid CartId { get; }
        public string ProductId { get; }
        public string ProductName { get; }
        public Money UnitPrice { get; }
        public int Quantity { get; }

        public CartProductAddedEvent(Guid cartId, string productId, string productName, Money unitPrice, int quantity)
        {
            CartId = cartId;
            ProductId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }
    }
}