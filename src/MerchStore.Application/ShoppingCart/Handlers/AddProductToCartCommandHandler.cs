using MediatR;
using MerchStore.Application.Common;
using MerchStore.Application.ShoppingCart.Commands;
using Microsoft.Extensions.Logging;

namespace MerchStore.Application.ShoppingCart.Handlers
{
    public class AddProductToCartCommandHandler : IRequestHandler<AddProductToCartCommand, Result<bool>>
    {
        private readonly ILogger<AddProductToCartCommandHandler> _logger;

        public AddProductToCartCommandHandler(ILogger<AddProductToCartCommandHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<bool>> Handle(AddProductToCartCommand request, CancellationToken cancellationToken)
        {
            // Validate productId
            if (string.IsNullOrEmpty(request.ProductId))
            {
                _logger.LogWarning("Validation failed: ProductId is null or empty.");
                return Result<bool>.Failure("Product ID cannot be null or empty.");
            }

            // Validate quantity
            if (request.Quantity <= 0)
            {
                _logger.LogWarning("Validation failed: Quantity must be greater than zero.");
                return Result<bool>.Failure("Quantity must be greater than zero.");
            }

            try
            {
                // Simulate adding the product to the cart
                _logger.LogInformation("Adding product {ProductId} to cart {CartId} with quantity {Quantity}.", request.ProductId, request.CartId, request.Quantity);

                // Business logic for adding the product to the cart would go here
                // (e.g., updating the cart in memory or database)

                _logger.LogInformation("Product {ProductId} added to cart {CartId} successfully.", request.ProductId, request.CartId);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding product {ProductId} to cart {CartId}.", request.ProductId, request.CartId);
                return Result<bool>.Failure("An unexpected error occurred while adding the product to the cart.");
            }
        }
    }
}