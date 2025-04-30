using System.Collections.Generic;

namespace MerchStore.Application.ShoppingCart.Dtos
{
    public class CartDto
    {
        public Guid Id { get; set; }
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        public decimal TotalPrice { get; set; }
        public int TotalItems { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}