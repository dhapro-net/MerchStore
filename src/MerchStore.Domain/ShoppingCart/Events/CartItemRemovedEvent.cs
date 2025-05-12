using System;
using MerchStore.Domain.Common;

namespace MerchStore.Domain.ShoppingCart.Events
{
    public class CartProductRemovedEvent : DomainEvent
    {
        public Guid CartId { get; }
        public string ProductId { get; }

        public CartProductRemovedEvent(Guid cartId, string productId)
        {
            CartId = cartId;
            ProductId = productId;
        }
    }
}