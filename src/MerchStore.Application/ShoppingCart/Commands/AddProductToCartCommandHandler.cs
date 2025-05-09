using MediatR;
using MerchStore.Application.Common;
using MerchStore.Application.ShoppingCart.Commands;
using MerchStore.Domain.ShoppingCart.Interfaces;
using MerchStore.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

public class AddProductToCartCommandHandler : IRequestHandler<AddProductToCartCommand, Result<bool>>
{
    private readonly IShoppingCartQueryRepository _queryRepository;
    private readonly IShoppingCartCommandRepository _commandRepository;
    private readonly ILogger<AddProductToCartCommandHandler> _logger;

    public AddProductToCartCommandHandler(
        IShoppingCartQueryRepository queryRepository,
        IShoppingCartCommandRepository commandRepository,
        ILogger<AddProductToCartCommandHandler> logger)
    {
        _queryRepository = queryRepository ?? throw new ArgumentNullException(nameof(queryRepository));
        _commandRepository = commandRepository ?? throw new ArgumentNullException(nameof(commandRepository));
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
            // Retrieve the cart using the query repository
            var cart = await _queryRepository.GetCartByIdAsync(request.CartId, cancellationToken);
            if (cart == null)
            {
                _logger.LogWarning("Cart with ID {CartId} not found.", request.CartId);
                return Result<bool>.Failure("Cart not found.");
            }

            // Add the product to the cart
            cart.AddProduct(request.ProductId, "Product Name", new Money(100, "SEK"), request.Quantity);

            // Save the updated cart using the command repository
            await _commandRepository.UpdateAsync(cart);

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