using System;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.Application.ShoppingCart.DTOs
{
    public class CartSummaryDto
    {
        public Guid CartId { get; set; }
        public int ProductCount { get; set; }
        public required Money TotalPrice { get; set; }
        
    }
}