using MerchStore.Domain.Entities;
using MerchStore.Domain.Interfaces;

namespace MerchStore.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for managing Order entities.
/// This class inherits from the generic Repository class and adds order-specific functionality.
/// </summary>
public class OrderRepository : Repository<Order, Guid>, IOrderRepository
{
    /// <summary>
    /// Constructor that passes the context to the base Repository class
    /// </summary>
    /// <param name="context">The database context</param>
    public OrderRepository(AppDbContext context) : base(context)
    {
    }
    //Add functions as needed.
}