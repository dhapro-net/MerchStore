
namespace MerchStore.Domain.Common
{
    /// <summary>
    /// Represents a base class for all domain events in the system.
    /// </summary>
    public abstract class DomainEvent : IDomainEvent
    {
        /// <summary>
        /// Gets the date and time when the event occurred.
        /// </summary>
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        /// <summary>
        /// Gets the unique identifier for the domain event.
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets the type of the domain event.
        /// </summary>
        public string EventType => GetType().Name;
    }
}