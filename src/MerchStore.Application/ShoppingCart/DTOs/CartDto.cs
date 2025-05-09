using MerchStore.Domain.ValueObjects;

namespace MerchStore.Application.ShoppingCart.DTOs
{
    public class CartDto
    {
        public Guid CartId { get; set; }
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        public Money TotalPrice { get; set; }
        public int TotalItems { get; set; }
        public DateTime LastUpdated { get; set; }

        // Method to calculate the total price
        public Money CalculateTotal()
        {
            var totalAmount = Items.Sum(item => (item.UnitPrice?.Amount ?? 0) * item.Quantity);
            return new Money(totalAmount, "SEK");
        }
    }
}