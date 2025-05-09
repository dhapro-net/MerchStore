using MediatR;
using MerchStore.Application.ShoppingCart.DTOs;

namespace MerchStore.Application.ShoppingCart.Queries;

/// <summary>
/// Query to retrieve a specific product from a shopping cart.
/// </summary>
public class GetProductQuery : IRequest<CartProductDto?>
{
    public Guid CartId { get; }
    public string ProductId { get; }

    public GetProductQuery(Guid cartId, string productId)
    {
        CartId = cartId;
        ProductId = productId;
    }
}