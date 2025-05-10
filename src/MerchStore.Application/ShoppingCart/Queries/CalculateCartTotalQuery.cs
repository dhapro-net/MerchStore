using MediatR;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.Application.ShoppingCart.Queries
{
    public class CalculateCartTotalQuery : IRequest<Money>
    {
        public Guid CartId { get; }

        public CalculateCartTotalQuery(Guid cartId)
        {
            CartId = cartId;
        }
    }
}