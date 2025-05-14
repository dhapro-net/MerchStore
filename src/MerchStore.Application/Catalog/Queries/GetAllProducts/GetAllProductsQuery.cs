using MediatR;

namespace MerchStore.Application.Catalog.Queries
{
    public class GetAllProductsQuery : IRequest<List<ProductDto>>
    {
    }
}