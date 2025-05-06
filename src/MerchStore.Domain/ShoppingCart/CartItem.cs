using System;

namespace MerchStore.Domain.ShoppingCart
{
    public class CartItem
    {
        public string ProductId { get; private set; }
        public string ProductName { get; private set; }
        public decimal UnitPrice { get; private set; }
        public int Quantity { get; private set; }
        public decimal TotalPrice => UnitPrice * Quantity;

        public CartItem(string productId, string productName, decimal unitPrice, int quantity)
        {
            if (string.IsNullOrEmpty(productId))
                throw new ArgumentNullException(nameof(productId));
            
            if (string.IsNullOrEmpty(productName))
                throw new ArgumentNullException(nameof(productName));
            
            if (unitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));
            
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));
            
            ProductId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }

        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(newQuantity));
            
            Quantity = newQuantity;
        }
    }
}