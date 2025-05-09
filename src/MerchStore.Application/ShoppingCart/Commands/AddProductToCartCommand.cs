using MediatR;
using MerchStore.Application.Common;

namespace MerchStore.Application.ShoppingCart.Commands;

/// <summary>
/// Represents a command to add a product to the shopping cart.
/// </summary>
public class AddProductToCartCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// Gets or sets the unique identifier of the shopping cart.
    /// </summary>
    public Guid CartId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the product to add.
    /// </summary>
    public string ProductId { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the product to add.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AddProductToCartCommand"/> class.
    /// </summary>
    /// <param name="cartId">The unique identifier of the shopping cart.</param>
    /// <param name="productId">The unique identifier of the product to add.</param>
    /// <param name="quantity">The quantity of the product to add.</param>
    public CancellationToken CancellationToken { get; }

    public AddProductToCartCommand(Guid cartId, string productId, int quantity)
    {
        CartId = cartId;
        ProductId = productId;
        Quantity = quantity;
    }
}