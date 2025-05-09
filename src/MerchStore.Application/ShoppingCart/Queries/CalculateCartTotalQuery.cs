using MediatR;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.Application.ShoppingCart.Queries;

/// <summary>
/// Represents a query to calculate the total price of a shopping cart.
/// </summary>
public class CalculateCartTotalQuery : IRequest<Money>
{
    /// <summary>
    /// Gets the unique identifier of the shopping cart.
    /// </summary>
    public Guid CartId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CalculateCartTotalQuery"/> class.
    /// </summary>
    /// <param name="cartId">The unique identifier of the shopping cart.</param>
    public CalculateCartTotalQuery(Guid cartId)
    {
        CartId = cartId;
    }
}