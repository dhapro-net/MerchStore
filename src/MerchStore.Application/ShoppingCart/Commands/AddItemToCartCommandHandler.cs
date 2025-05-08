using MediatR;
using MerchStore.Application.Common;
using MerchStore.Application.ShoppingCart.Commands;
using MerchStore.Application.ShoppingCart.Interfaces;

namespace MerchStore.Application.ShoppingCart.Commands
{
    public class AddItemToCartCommandHandler : IRequestHandler<AddItemToCartCommand, Result<bool>>
    {
        private readonly IShoppingCartService _cartService;
        
        public AddItemToCartCommandHandler(IShoppingCartService cartService)
        {
            _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
        }
        
        public async Task<Result<bool>> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
        {
            if (request.CartId == Guid.Empty)
                return Result.Failure<bool>("Cart ID cannot be empty");
                
            if (string.IsNullOrEmpty(request.ProductId))
                return Result.Failure<bool>("Product ID cannot be empty");
                
            if (request.Quantity <= 0)
                return Result.Failure<bool>("Quantity must be greater than zero");
                
            var success = await _cartService.AddItemToCartAsync(
                request.CartId,
                request.ProductId,
                request.Quantity);
                
            if (!success)
                return Result.Failure<bool>("Failed to add item to cart. The product may not exist or is unavailable.");
                
            return Result.Success(true);
        }
    }
}