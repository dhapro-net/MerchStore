using System;
using MerchStore.Domain.Common;

namespace MerchStore.Domain.ShoppingCart.Events
{
    public class CartClearedEvent : DomainEvent
    {
        public Guid CartId { get; }

        public CartClearedEvent(Guid cartId)
        {
            CartId = cartId;
        }
    }
}