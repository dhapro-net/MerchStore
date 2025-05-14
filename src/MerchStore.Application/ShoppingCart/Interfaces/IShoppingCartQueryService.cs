using MerchStore.Application.ShoppingCart.DTOs;
using MerchStore.Application.ShoppingCart.Queries;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.Application.ShoppingCart.Interfaces;

/// <summary>
/// Defines query operations for retrieving shopping cart data.
/// </summary>
public interface IShoppingCartQueryService
{

    /// <summary>
    /// Calculates the total price of the shopping cart.
    /// </summary>
    /// <param name="cartId">The unique identifier of the shopping cart.</param>
    /// <returns>The total price of the shopping cart as a <see cref="Money"/> object.</returns>
    Task<Money> CalculateCartTotalAsync(Guid cartId);

    /// <summary>
    /// Retrieves the shopping cart details.
    /// </summary>
    /// <param name="query">The query containing the cart identifier.</param>
    /// <returns>The shopping cart details as a <see cref="CartDto"/>.</returns>
    Task<CartDto> GetCartAsync(GetCartQuery query, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a summary of the shopping cart.
    /// </summary>
    /// <param name="query">The query containing the cart identifier.</param>
    /// <returns>The shopping cart summary as a <see cref="CartSummaryDto"/>.</returns>
    Task<CartSummaryDto> GetCartSummaryAsync(GetCartSummaryQuery query, CancellationToken cancellationToken);
}