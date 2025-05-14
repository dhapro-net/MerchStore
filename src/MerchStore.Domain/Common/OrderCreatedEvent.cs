using MerchStore.Domain.Common;
using MerchStore.Domain.Entities;

namespace MerchStore.Domain.Events
{
    public class OrderCreatedEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; }
        public Order Order { get; }

        public OrderCreatedEvent(Order order)
        {
            Order = order ?? throw new ArgumentNullException(nameof(order));
            OccurredOn = DateTime.UtcNow; 
        }
    }
}