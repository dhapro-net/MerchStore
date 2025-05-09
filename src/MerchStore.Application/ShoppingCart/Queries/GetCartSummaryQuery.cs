using MediatR;
using MerchStore.Application.ShoppingCart.DTOs;

namespace MerchStore.Application.ShoppingCart.Queries;

/// <summary>
/// Query to retrieve the summary of a shopping cart.
/// </summary>
public class GetCartSummaryQuery : IRequest<CartSummaryDto>
{
    /// <summary>
    /// Gets the unique identifier of the shopping cart.
    /// </summary>
    public Guid CartId { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCartSummaryQuery"/> class.
    /// </summary>
    /// <param name="cartId">The unique identifier of the shopping cart.</param>
    public GetCartSummaryQuery(Guid cartId)
    {
        CartId = cartId;
    }
}