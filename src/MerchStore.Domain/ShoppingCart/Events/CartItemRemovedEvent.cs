using System;
using MerchStore.Domain.Common;

namespace MerchStore.Domain.ShoppingCart.Events
{
    /// <summary>
    /// Represents the event that occurs when a product is removed from a shopping cart.
    /// </summary>
    public class CartProductRemovedEvent : DomainEvent
    {
        /// <summary>
        /// Gets the unique identifier of the cart.
        /// </summary>
        public Guid CartId { get; }

        /// <summary>
        /// Gets the unique identifier of the product.
        /// </summary>
        public string ProductId { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CartProductRemovedEvent"/> class.
        /// </summary>
        /// <param name="cartId">The unique identifier of the cart.</param>
        /// <param name="productId">The unique identifier of the product.</param>
        /// <exception cref="ArgumentException">Thrown when any parameter is invalid.</exception>
        public CartProductRemovedEvent(Guid cartId, string productId)
        {
            if (cartId == Guid.Empty)
            {
                throw new ArgumentException("Cart ID cannot be empty.", nameof(cartId));
            }

            if (string.IsNullOrWhiteSpace(productId))
            {
                throw new ArgumentException("Product ID cannot be null or empty.", nameof(productId));
            }

            CartId = cartId;
            ProductId = productId;
        }
    }
}