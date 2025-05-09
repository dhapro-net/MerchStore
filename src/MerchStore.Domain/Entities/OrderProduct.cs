using MerchStore.Domain.ValueObjects;

namespace MerchStore.Domain.Entities;

public class OrderProduct
{
    public Guid Id { get; set; } // Primary Key
    public Guid ProductId { get; set; } // Foreign Key to Product
    public string ProductName { get; set; }
    public Money UnitPrice { get; set; }
    public int Quantity { get; set; }
    public Money TotalPrice => new Money(UnitPrice.Amount * Quantity, UnitPrice.Currency);

    // Foreign key to the Order
    public Guid OrderId { get; set; }
    public Order Order { get; set; }
}