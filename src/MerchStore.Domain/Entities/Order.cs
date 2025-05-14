using MerchStore.Domain.Common;
using MerchStore.Domain.Events;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.Domain.Entities;

public class Order : Entity<Guid>
{
    private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        if (domainEvent == null) throw new ArgumentNullException(nameof(domainEvent));
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        if (domainEvent == null) throw new ArgumentNullException(nameof(domainEvent));
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
    public PaymentInfo PaymentInfo { get; private set; }
    public string CustomerName { get; private set; }
    public string Address { get; private set; }
    public Money TotalPrice { get; private set; }
    public List<OrderProduct> Products { get; private set; } = new List<OrderProduct>();
    public DateTime CreatedDate { get; private set; }

    private Order() { }

    public Order(Guid id, PaymentInfo paymentInfo, string customerName, string address, Money totalPrice, List<OrderProduct> products, DateTime createdDate)
    {
        Id = id; // Provided by Entity<Guid>
        PaymentInfo = paymentInfo ?? throw new ArgumentNullException(nameof(paymentInfo));
        CustomerName = customerName ?? throw new ArgumentNullException(nameof(customerName));
        Address = address ?? throw new ArgumentNullException(nameof(address));
        TotalPrice = totalPrice ?? throw new ArgumentNullException(nameof(totalPrice));
        CreatedDate = createdDate;

        // Step 2: Validate the Products list
        if (products == null || !products.Any())
        {
            throw new ArgumentException("An order must contain at least one product.", nameof(products));
        }
        Products = products;

        // Step 4: Raise a domain event for order creation
        AddDomainEvent(new OrderCreatedEvent(this));
    }

    // Step 3: Add a method to encapsulate adding a product
    public void AddProduct(OrderProduct product)
    {
        if (product == null)
        {
            throw new ArgumentNullException(nameof(product), "Product cannot be null.");
        }

        Products.Add(product);

        // Optionally, recalculate the total price
        TotalPrice = new Money(Products.Sum(p => p.UnitPrice.Amount * p.Quantity), TotalPrice.Currency);
    }
}