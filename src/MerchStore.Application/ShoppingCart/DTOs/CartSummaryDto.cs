using System;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.Application.ShoppingCart.DTOs
{
    public class CartSummaryDto
    {
        public Guid CartId { get; set; }
        public int ItemsCount { get; set; }
        public Money TotalPrice { get; set; }
        
    }
}