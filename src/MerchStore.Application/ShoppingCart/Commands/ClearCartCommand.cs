using MediatR;
using MerchStore.Application.Common;

namespace MerchStore.Application.ShoppingCart.Commands;

/// <summary>
/// Represents a command to clear the shopping cart.
/// </summary>
public class ClearCartCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// Gets or sets the unique identifier of the shopping cart to clear.
    /// </summary>
    public Guid CartId { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClearCartCommand"/> class.
    /// </summary>
    /// <param name="cartId">The unique identifier of the shopping cart to clear.</param>
    public ClearCartCommand(Guid cartId)
    {
        CartId = cartId;
    }
}