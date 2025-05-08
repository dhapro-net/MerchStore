using System;

namespace MerchStore.Application.ShoppingCart.DTOs
{
    public class CartSummaryDto
    {
        public Guid CartId { get; set; }
        public int ItemsCount { get; set; }
        public decimal TotalPrice { get; set; }
    }
}