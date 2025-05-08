using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MerchStore.Application.ShoppingCart.Dtos;

namespace MerchStore.Application.ShoppingCart.Queries
{
    public class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartDto>
    {
        public async Task<CartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            // Logic to retrieve the cart by CartId
            // Example: Fetch from database or in-memory store
            return new CartDto
            {
                CartId = request.CartId,
                Items = new List<CartItemDto>() // Populate with actual data
            };
        }
    }
}