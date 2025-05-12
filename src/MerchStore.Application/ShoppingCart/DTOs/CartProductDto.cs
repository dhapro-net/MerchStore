using MerchStore.Domain.ValueObjects;

namespace MerchStore.Application.ShoppingCart.DTOs
{
    public class CartProductDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public Money UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}