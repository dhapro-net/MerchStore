using MerchStore.Domain.Common;

// Could do with an eventdispatcher
namespace MerchStore.Domain.ShoppingCart.Events
{
    /// <summary>
    /// Represents the event that occurs when a shopping cart is cleared.
    /// </summary>
    public class CartClearedEvent : DomainEvent
    {
        /// <summary>
        /// Gets the unique identifier of the cleared cart.
        /// </summary>
        public Guid CartId { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CartClearedEvent"/> class.
        /// </summary>
        /// <param name="cartId">The unique identifier of the cleared cart.</param>
        /// <exception cref="ArgumentException">Thrown when the cart ID is empty.</exception>
        public CartClearedEvent(Guid cartId)
        {
            if (cartId == Guid.Empty)
            {
                throw new ArgumentException("Cart ID cannot be empty.", nameof(cartId));
            }

            CartId = cartId;
        }
    }
}