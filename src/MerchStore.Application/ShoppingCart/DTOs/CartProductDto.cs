using MerchStore.Domain.ValueObjects;

namespace MerchStore.Application.ShoppingCart.DTOs
{
    public class CartProductDto
    {
        public required string ProductId { get; set; }
        public required string ProductName { get; set; }
        public required Money UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}