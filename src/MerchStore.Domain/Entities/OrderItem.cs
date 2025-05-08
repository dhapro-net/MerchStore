using MerchStore.Domain.ValueObjects;

namespace MerchStore.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; set; } // Primary Key
    public Guid ProductId { get; set; } // Foreign Key to Product
    public string ProductName { get; set; }
    public Money Price { get; set; }
    public int Quantity { get; set; }
    public Money TotalPrice => new Money(Price.Amount * Quantity, Price.Currency);

    // Foreign key to the Order
    public Guid OrderId { get; set; }
    public Order Order { get; set; }
}