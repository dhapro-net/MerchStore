using MediatR;
using MerchStore.Application.ShoppingCart.DTOs;
using MerchStore.Domain.ShoppingCart.Interfaces;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.Application.ShoppingCart.Queries
{
    public class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartDto>
    {
        private readonly IShoppingCartRepository _repository;

        public GetCartQueryHandler(IShoppingCartRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<CartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            // Fetch the cart from the repository
            var cart = await _repository.GetCartByIdAsync(request.CartId, cancellationToken);

            // If the cart is null, return a new CartDto with default values
            if (cart == null)
            {
                return new CartDto
                {
                    CartId = request.CartId,
                    Products = new List<CartProductDto>(),
                    TotalPrice = new Money(0, "SEK"),
                    TotalProducts = 0,
                    LastUpdated = DateTime.UtcNow
                };
            }

            // Map the cart to CartDto
            return new CartDto
{
    CartId = cart.Id,
    Products = cart.Products.Select(product => new CartProductDto // Fixed: Changed 'Product' to 'Products'
    {
        ProductId = product.ProductId,
        ProductName = product.ProductName,
        UnitPrice = product.UnitPrice,
        Quantity = product.Quantity
    }).ToList(),
    TotalPrice = cart.CalculateTotal(),
    TotalProducts = cart.Products.Sum(i => i.Quantity), // Fixed: Changed 'Product' to 'Products'
    LastUpdated = cart.LastUpdated
};;
        }
    }
}