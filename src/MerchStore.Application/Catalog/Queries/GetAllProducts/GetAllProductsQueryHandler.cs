using MediatR;
using MerchStore.Domain.Interfaces;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.Application.Catalog.Queries
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<ProductDto>>
    {
        private readonly IProductQueryRepository _repository;

        public GetAllProductsQueryHandler(IProductQueryRepository repository)
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
                Price = new Money(p.Price.Amount, p.Price.Currency), 
                ImageUrl = string.IsNullOrEmpty(p.ImageUrl) ? null : new Uri(p.ImageUrl), 
                StockQuantity = p.StockQuantity
            }).ToList();
        }
    }
}