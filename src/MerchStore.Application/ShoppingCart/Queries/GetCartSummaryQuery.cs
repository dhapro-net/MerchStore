using MediatR;
using MerchStore.Application.ShoppingCart.DTOs;

namespace MerchStore.Application.ShoppingCart.Queries
{
    public class GetCartSummaryQuery : IRequest<CartSummaryDto>
    {
        public Guid CartId { get; set; }
            public GetCartSummaryQuery(Guid cartId)
    {
        CartId = cartId;
    }
    }
    
}