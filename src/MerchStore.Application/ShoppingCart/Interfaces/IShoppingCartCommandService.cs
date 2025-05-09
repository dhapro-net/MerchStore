using MerchStore.Domain.ShoppingCart;

namespace MerchStore.Application.ShoppingCart.Interfaces;

/// <summary>
/// Defines command operations for managing the shopping cart.
/// </summary>
public interface IShoppingCartCommandService
{
    /// <summary>
    /// Adds a product to the shopping cart.
    /// </summary>
    /// <param name="cartId">The unique identifier of the shopping cart.</param>
    /// <param name="productId">The unique identifier of the product to add.</param>
    /// <param name="quantity">The quantity of the product to add.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns><c>true</c> if the product was added successfully; otherwise, <c>false</c>.</returns>
    Task<bool> AddProductToCartAsync(Guid cartId, string productId, int quantity, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a product from the shopping cart.
    /// </summary>
    /// <param name="cartId">The unique identifier of the shopping cart.</param>
    /// <param name="productId">The unique identifier of the product to remove.</param>
    /// <returns><c>true</c> if the product was removed successfully; otherwise, <c>false</c>.</returns>
    Task<bool> RemoveProductFromCartAsync(Guid cartId, string productId);

    /// <summary>
    /// Updates the quantity of a product in the shopping cart.
    /// </summary>
    /// <param name="cartId">The unique identifier of the shopping cart.</param>
    /// <param name="productId">The unique identifier of the product to update.</param>
    /// <param name="quantity">The new quantity of the product.</param>
    /// <returns><c>true</c> if the quantity was updated successfully; otherwise, <c>false</c>.</returns>
    Task<bool> UpdateProductQuantityAsync(Guid cartId, string productId, int quantity);

    /// <summary>
    /// Clears all products from the shopping cart.
    /// </summary>
    /// <param name="cartId">The unique identifier of the shopping cart.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns><c>true</c> if the cart was cleared successfully; otherwise, <c>false</c>.</returns>
    Task<bool> ClearCartAsync(Guid cartId, CancellationToken cancellationToken);

    Task AddAsync(Cart cart, CancellationToken cancellationToken);


}