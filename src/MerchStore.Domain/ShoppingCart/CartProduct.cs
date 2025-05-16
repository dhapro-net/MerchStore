using MerchStore.Domain.ValueObjects;

namespace MerchStore.Domain.ShoppingCart
{
 
    public class CartProduct
    {

        public string ProductId { get; private set; }


        public string ProductName { get; private set; }


        public Money UnitPrice { get; private set; }

 
        public int Quantity { get; private set; }


        public Money TotalPrice => UnitPrice * Quantity;


        /// Initializes a new instance of the <see cref="CartProduct"/> class.


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