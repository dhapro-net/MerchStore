using MediatR;
using MerchStore.Application;
using MerchStore.Domain;
using MerchStore.Domain.Interfaces;

namespace MerchStore.Application.Catalog.Queries
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IProductQueryRepository _repository;

        public GetProductByIdQueryHandler(IProductQueryRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetProductByIdAsync(request.ProductId, cancellationToken);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {request.ProductId} not found.");
            }

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = string.IsNullOrEmpty(product.ImageUrl)
    ? new Uri("https://example.com/placeholder.jpg") // Use a sensible default
    : new Uri(product.ImageUrl),

                StockQuantity = product.StockQuantity
            };
        }
    }
}