using MediatR;
using MerchStore.Application.Common;
using MerchStore.Application.ShoppingCart.Commands;
using MerchStore.Application.ShoppingCart.Interfaces;

namespace MerchStore.Application.ShoppingCart.Commands
{
    public class AddProductToCartCommandHandler : IRequestHandler<AddProductToCartCommand, Result<bool>>
    {
        private readonly IShoppingCartService _cartService;
        
        public AddProductToCartCommandHandler(IShoppingCartService cartService)
        {
            _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
        }
        
        public async Task<Result<bool>> Handle(AddProductToCartCommand request, CancellationToken cancellationToken)
        {
            if (request.CartId == Guid.Empty)
                return Result.Failure<bool>("Cart ID cannot be empty");
                
            if (string.IsNullOrEmpty(request.ProductId))
                return Result.Failure<bool>("Product ID cannot be empty");
                
            if (request.Quantity <= 0)
                return Result.Failure<bool>("Quantity must be greater than zero");
                
            var success = await _cartService.AddProductToCartAsync(
                request.CartId,
                request.ProductId,
                request.Quantity,
                cancellationToken);
                
            if (!success)
                return Result.Failure<bool>("Failed to add product to cart. The product may not exist or is unavailable.");
                
            return Result.Success(true);
        }
    }
}