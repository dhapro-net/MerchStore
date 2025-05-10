using MerchStore.Domain.ValueObjects;
using MerchStore.Domain.ShoppingCart;

namespace MerchStore.Domain.Services;

/// <summary>
/// Service for calculating totals for a shopping cart.
/// </summary>
public class CartCalculationService
{
    private const string DefaultCurrency = "SEK";

    /// <summary>
    /// Calculates the total price of the products in the shopping cart.
    /// </summary>
    /// <param name="products">The list of products in the shopping cart.</param>
    /// <returns>The total price as a Money value object.</returns>
    public Money CalculateTotal(List<CartProduct> products)
    {
        if (products == null || !products.Any())
        {
            return new Money(0, DefaultCurrency);
        }

        var totalAmount = products.Sum(product => product.UnitPrice.Amount * product.Quantity);
        return new Money(totalAmount, DefaultCurrency);
    }
}