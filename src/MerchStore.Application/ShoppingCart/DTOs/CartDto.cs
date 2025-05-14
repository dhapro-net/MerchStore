using MerchStore.Domain.ValueObjects;

namespace MerchStore.Application.ShoppingCart.DTOs
{
    public class CartDto
    {
        public Guid CartId { get; set; }
        public List<CartProductDto> Products { get; set; } = new List<CartProductDto>();
        public Money TotalPrice { get; set; }
        public int TotalProducts { get; set; }
        public DateTime LastUpdated { get; set; }

    }
}