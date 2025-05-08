using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MerchStore.Application.ShoppingCart.DTOs;
using MerchStore.Application.Common.Interfaces;

namespace MerchStore.Application.ShoppingCart.Queries
{
    public class GetCartSummaryQueryHandler : IRequestHandler<GetCartSummaryQuery, CartSummaryDto>
    {
        private readonly ICartRepository _cartRepository;

        public GetCartSummaryQueryHandler(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<CartSummaryDto> Handle(GetCartSummaryQuery request, CancellationToken cancellationToken)
        {
            // Fetch the cart from the repository
            var cart = await _cartRepository.GetCartByIdAsync(request.CartId, cancellationToken);

            if (cart == null)
            {
                throw new NotFoundException($"Cart with ID {request.CartId} not found.");
            }

            // Map the cart to a summary DTO
            return new CartSummaryDto
            {
                CartId = cart.Id,
                TotalItems = cart.Items.Count,
                TotalPrice = cart.Items.Sum(item => item.Price * item.Quantity)
            };
        }
    }
}