
namespace MerchStore.Domain.Common
{
    public abstract class AggregateRoot<TId> : Entity<TId> where TId : notnull
    {
        private readonly List<DomainEvent> _domainEvents = new List<DomainEvent>();

        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected AggregateRoot(TId id) : base(id) { }

        // Required for EF Core
        protected AggregateRoot() { }

        protected void AddDomainEvent(DomainEvent domainEvent)
        {
            if (domainEvent == null)
            {
                throw new ArgumentNullException(nameof(domainEvent), "Domain event cannot be null.");
            }

            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}