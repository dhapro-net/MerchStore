using MediatR;
using MerchStore.Application.Common;

namespace MerchStore.Application.ShoppingCart.Commands;

/// <summary>
/// Represents a command to add a product to the shopping cart.
/// </summary>
public class AddProductToCartCommand : IRequest<Result<bool>>
{
    public Guid CartId { get; }
    public string ProductId { get; }
    public int Quantity { get; }
    public CancellationToken CancellationToken { get; }

    public AddProductToCartCommand(Guid cartId, string productId, int quantity, CancellationToken cancellationToken)
    {
        CartId = cartId;
        ProductId = productId;
        Quantity = quantity;
        CancellationToken = cancellationToken;
    }
}
