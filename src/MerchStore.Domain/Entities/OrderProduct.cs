using MerchStore.Domain.ValueObjects;

namespace MerchStore.Domain.Entities;

public class OrderProduct
{
    public Guid Id { get; private set; } // Primary Key
    public Guid ProductId { get; private set; } // Foreign Key to Product
    public string ProductName { get; private set; }
    public Money UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public Money TotalPrice => new Money(UnitPrice.Amount * Quantity, UnitPrice.Currency);

    // Foreign key to the Order
    public Guid OrderId { get; private set; }
    public Order Order { get; private set; }

    public OrderProduct(Guid id, Guid productId, string productName, Money unitPrice, int quantity, Guid orderId)
    {
        Id = id;
        ProductId = productId;
        ProductName = productName ?? throw new ArgumentNullException(nameof(productName));
        UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
        Quantity = quantity > 0 ? quantity : throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        OrderId = orderId;
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