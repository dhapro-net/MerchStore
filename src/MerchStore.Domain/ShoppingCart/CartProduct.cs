using System;
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

        public CartProduct(string productId, string productName, Money unitPrice, int quantity)
        {
            if (string.IsNullOrEmpty(productId))
                throw new ArgumentNullException(nameof(productId));
            
            if (string.IsNullOrEmpty(productName))
                throw new ArgumentNullException(nameof(productName));
            
            if (unitPrice == null)
                throw new ArgumentNullException(nameof(unitPrice));
            
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