using MerchStore.Domain.ValueObjects;

namespace MerchStore.Domain.Entities;

public class OrderProduct
{
    public Guid Id { get; private set; } // Primary Key
    public Guid ProductId { get; private set; } // Foreign Key to Product
    public string ProductName { get; private set; }  = null!;
    public Money UnitPrice { get; private set; } = null!;
    public int Quantity { get; private set; }
    public Money TotalPrice => new Money(UnitPrice.Amount * Quantity, UnitPrice.Currency);

    // Foreign key to the Order
    public Guid OrderId { get; private set; }
    public Order? Order { get; private set; }
    public Guid Guid { get; }
    public string? V1 { get; }
    public Money? Money { get; }
    public int V2 { get; }

    private OrderProduct() { } //For EF Core 
    public OrderProduct(Guid id, Guid productId, string productName, Money unitPrice, int quantity, Guid orderId)
    {
        Id = id;
        ProductId = productId;
        ProductName = productName ?? throw new ArgumentNullException(nameof(productName));
        UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
        Quantity = quantity > 0 ? quantity : throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        OrderId = orderId;
    }

    public OrderProduct(Guid guid, string v1, Money money, int v2)
    {
        Guid = guid;
        V1 = v1;
        Money = money;
        V2 = v2;
    }

    // Optional: Method to update quantity
    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.", nameof(newQuantity));
        }

        Quantity = newQuantity;
    }
}