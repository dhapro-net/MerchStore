using MediatR;
using MerchStore.Application.DTOs;

namespace MerchStore.Application.Catalog.Queries
{
    public class GetProductByIdQuery : IRequest<ProductDto>
    {
        public Guid ProductId { get; }

        public GetProductByIdQuery(Guid productId)
        {
            ProductId = productId;
        }
    }
}