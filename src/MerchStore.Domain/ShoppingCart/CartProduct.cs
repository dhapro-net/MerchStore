using System;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.Domain.ShoppingCart
{
    /// <summary>
    /// Represents a product in the shopping cart.
    /// </summary>
    public class CartProduct
    {
        /// <summary>
        /// Gets the unique identifier of the product.
        /// </summary>
        public string ProductId { get; private set; }

        /// <summary>
        /// Gets the name of the product.
        /// </summary>
        public string ProductName { get; private set; }

        /// <summary>
        /// Gets the unit price of the product.
        /// </summary>
        public Money UnitPrice { get; private set; }

        /// <summary>
        /// Gets the quantity of the product in the cart.
        /// </summary>
        public int Quantity { get; private set; }

        /// <summary>
        /// Gets the total price for the product in the cart.
        /// </summary>
        public Money TotalPrice => UnitPrice * Quantity;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartProduct"/> class.
        /// </summary>
        /// <param name="productId">The unique identifier of the product.</param>
        /// <param name="productName">The name of the product.</param>
        /// <param name="unitPrice">The unit price of the product.</param>
        /// <param name="quantity">The quantity of the product in the cart.</param>
        /// <exception cref="ArgumentNullException">Thrown when any required parameter is null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown when the quantity is not positive.</exception>
        public CartProduct(string productId, string productName, Money unitPrice, int quantity)
        {
            if (string.IsNullOrEmpty(productId))
                throw new ArgumentNullException(nameof(productId), "Product ID cannot be null or empty.");
            
            if (string.IsNullOrEmpty(productName))
                throw new ArgumentNullException(nameof(productName), "Product name cannot be null or empty.");
            
            if (unitPrice == null)
                throw new ArgumentNullException(nameof(unitPrice), "Unit price cannot be null.");
            
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive.", nameof(quantity));
            
            ProductId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }

        /// <summary>
        /// Updates the quantity of the product in the cart.
        /// </summary>
        /// <param name="newQuantity">The new quantity of the product.</param>
        /// <exception cref="ArgumentException">Thrown when the new quantity is not positive.</exception>
        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new ArgumentException("Quantity must be positive.", nameof(newQuantity));
            
            Quantity = newQuantity;
        }
    }
}