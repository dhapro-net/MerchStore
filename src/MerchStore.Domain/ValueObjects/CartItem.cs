using System;

namespace MerchStore.Domain.ShoppingCart
{
    public class CartItem
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        
        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(newQuantity));
                
            Quantity = newQuantity;
        }
        
        public decimal CalculateTotal() => UnitPrice * Quantity;
    }
}