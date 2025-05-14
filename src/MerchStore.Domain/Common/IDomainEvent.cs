using System.Collections.Generic;
namespace MerchStore.Domain.Common;

/// <summary>
/// Represents a domain event in the system.
/// Domain events are used to signal that something significant has occurred within the domain.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    DateTime OccurredOn { get; }

    
}