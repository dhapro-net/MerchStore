using MerchStore.Domain.Common;
using MerchStore.Domain.ValueObjects;


//Could use and eventdispatcher
namespace MerchStore.Domain.ShoppingCart.Events
{
    /// <summary>
    /// Represents the event that occurs when a product is added to a shopping cart.
    /// </summary>
    public class CartProductAddedEvent : DomainEvent
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
        /// Gets the name of the product.
        /// </summary>
        public string ProductName { get; }

        /// <summary>
        /// Gets the unit price of the product.
        /// </summary>
        public Money UnitPrice { get; }

        /// <summary>
        /// Gets the quantity of the product added to the cart.
        /// </summary>
        public int Quantity { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CartProductAddedEvent"/> class.
        /// </summary>
        /// <param name="cartId">The unique identifier of the cart.</param>
        /// <param name="productId">The unique identifier of the product.</param>
        /// <param name="productName">The name of the product.</param>
        /// <param name="unitPrice">The unit price of the product.</param>
        /// <param name="quantity">The quantity of the product added to the cart.</param>
        /// <exception cref="ArgumentException">Thrown when any parameter is invalid.</exception>
        public CartProductAddedEvent(Guid cartId, string productId, string productName, Money unitPrice, int quantity)
        {
            if (cartId == Guid.Empty)
            {
                throw new ArgumentException("Cart ID cannot be empty.", nameof(cartId));
            }

            if (string.IsNullOrWhiteSpace(productId))
            {
                throw new ArgumentException("Product ID cannot be null or empty.", nameof(productId));
            }

            if (string.IsNullOrWhiteSpace(productName))
            {
                throw new ArgumentException("Product name cannot be null or empty.", nameof(productName));
            }

            if (unitPrice == null)
            {
                throw new ArgumentNullException(nameof(unitPrice), "Unit price cannot be null.");
            }

            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
            }

            CartId = cartId;
            ProductId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }
    }
}