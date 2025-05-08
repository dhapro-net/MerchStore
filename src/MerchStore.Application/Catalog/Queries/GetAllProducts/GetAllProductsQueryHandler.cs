using MediatR;
using MerchStore.Application.DTOs;
using MerchStore.Domain.Catalog.Interfaces;

namespace MerchStore.Application.Catalog.Queries
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<ProductDto>>
    {
        private readonly ICatalogRepository _repository;

        public GetAllProductsQueryHandler(ICatalogRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<List<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _repository.GetAllProductsAsync(cancellationToken);
            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                StockQuantity = p.StockQuantity
            }).ToList();
        }
    }
}