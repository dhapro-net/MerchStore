using MerchStore.Domain.ValueObjects;

namespace MerchStore.Domain.Entities;

public class Order
{
    public Guid Id { get; set; } // Primary Key
    public string CustomerName { get; set; }
    public string Address { get; set; }
    public Money TotalPrice { get; set; }

    // Navigation property for order items
    public List<OrderItem> Items { get; set; }

    // Payment information
    public PaymentInfo Payment { get; set; }
}