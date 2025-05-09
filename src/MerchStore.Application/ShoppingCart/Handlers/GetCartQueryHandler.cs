using MediatR;
using MerchStore.Application.ShoppingCart.DTOs;
using MerchStore.Application.ShoppingCart.Queries;
using MerchStore.Domain.ShoppingCart;
using MerchStore.Domain.ShoppingCart.Interfaces;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.Application.ShoppingCart.Handlers;

/// <summary>
/// Handles the GetCartQuery.
/// </summary>
public class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartDto>
{
    private readonly IShoppingCartQueryRepository _repository;

    public GetCartQueryHandler(IShoppingCartQueryRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<CartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var cart = await _repository.GetCartByIdAsync(request.CartId, cancellationToken);
        if (cart == null)
        {
            return null; // Or handle the null case appropriately
        }

        // Map Cart to CartDto
        return new CartDto
        {
            CartId = cart.CartId,
            Products = cart.Products.Select(p => new CartProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                UnitPrice = p.UnitPrice,
                Quantity = p.Quantity
            }).ToList(),
            TotalPrice = new Money(cart.Products.Sum(p => p.UnitPrice.Amount * p.Quantity), "SEK"),
            LastUpdated = cart.LastUpdated
        };
    }
}