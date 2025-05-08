using MediatR;
using MerchStore.Application.DTOs;

namespace MerchStore.Application.Catalog.Queries
{
    public class GetAllProductsQuery : IRequest<List<ProductDto>>
    {
    }
}