using MerchStore.Domain.Common;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.Domain.Entities;

public class Order : Entity<Guid>
{
    public PaymentInfo PaymentInfo { get; private set; }
    public string CustomerName { get; private set; }
    public string Address { get; private set; }
    public Money TotalPrice { get; private set; }
    public List<OrderProduct> Products { get; private set; } = new List<OrderProduct>();
    public DateTime CreatedDate { get; private set; }

    private Order() { }

    public Order(Guid id, PaymentInfo paymentInfo, string customerName, string address, Money totalPrice, DateTime createdDate)
    {
        Id = id; // Provided by Entity<Guid>
        PaymentInfo = paymentInfo ?? throw new ArgumentNullException(nameof(paymentInfo));
        CustomerName = customerName ?? throw new ArgumentNullException(nameof(customerName));
        Address = address ?? throw new ArgumentNullException(nameof(address));
        TotalPrice = totalPrice ?? throw new ArgumentNullException(nameof(totalPrice));
        CreatedDate = createdDate;
    }
}