using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MerchStore.Application.ShoppingCart.DTOs;
using MerchStore.Application.Common.Interfaces;
using MerchStore.Application.ShoppingCart.Interfaces;
using MerchStore.Domain.ShoppingCart.Interfaces;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.Application.ShoppingCart.Queries
{
    public class GetCartSummaryQueryHandler : IRequestHandler<GetCartSummaryQuery, CartSummaryDto>
    {
        private readonly IShoppingCartRepository _cartRepository;

        public GetCartSummaryQueryHandler(IShoppingCartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<CartSummaryDto> Handle(GetCartSummaryQuery request, CancellationToken cancellationToken)
        {
            // Fetch the cart from the repository
            var cart = await _cartRepository.GetCartByIdAsync(request.CartId, cancellationToken);

            if (cart == null)
            {
                throw new InvalidOperationException($"Cart with ID {request.CartId} not found.");
            }

            // Map the cart to a summary DTO
            return new CartSummaryDto
            {
                CartId = cart.Id,
                ItemsCount = cart.Items.Count,
                TotalPrice = new Money(cart.Items.Sum(item => (item.UnitPrice?.Amount ?? 0) * item.Quantity), "SEK")
            };
        }
    }
}