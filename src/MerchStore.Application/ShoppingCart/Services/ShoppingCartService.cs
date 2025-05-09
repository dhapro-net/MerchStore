using MediatR;
using MerchStore.Application.ShoppingCart.Commands;
using MerchStore.Application.ShoppingCart.DTOs;
using MerchStore.Application.ShoppingCart.Interfaces;
using MerchStore.Application.ShoppingCart.Queries;
using MerchStore.Domain.Interfaces;
using MerchStore.Domain.ShoppingCart;
using MerchStore.Domain.ShoppingCart.Interfaces;
using MerchStore.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace MerchStore.Application.ShoppingCart.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ShoppingCartService> _logger;
        private readonly IShoppingCartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILoggerFactory _loggerFactory;


        public ShoppingCartService(
            IMediator mediator,
            IShoppingCartRepository cartRepository,
            IProductRepository productRepository,
            ILogger<ShoppingCartService> logger,
            ILoggerFactory loggerFactory)
        {

            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }


        public async Task<CartDto> GetOrCreateCartAsync(Guid cartId, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Fetching or creating cart with ID: {cartId}.");

            var cart = await _cartRepository.GetCartByIdAsync(cartId, cancellationToken);
            if (cart == null)
            {
                _logger.LogWarning($"Cart with ID {cartId} not found. Creating a new cart.");

                // Use a logging scope for cart creation
                using (_logger.BeginScope("Cart Creation Scope"))
                {
                    var cartLogger = _loggerFactory.CreateLogger<Cart>(); // Create an ILogger<Cart>
                    cart = Cart.Create(cartId, cartLogger);
                    await _cartRepository.AddAsync(cart);
                }
            }

            return new CartDto
            {
                CartId = cart.CartId,
                Items = cart.Items.Select(i => new CartItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity
                }).ToList()
            };
        }
        public async Task<Money> CalculateCartTotalAsync(Guid cartId)
        {
            _logger.LogInformation($"Calculating total for cart with ID: {cartId}.");
            return await _mediator.Send(new CalculateCartTotalQuery(cartId));
        }

        public async Task<bool> AddItemToCartAsync(Guid cartId, string productId, int quantity, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Adding ProductId: {productId} with Quantity: {quantity} to CartId: {cartId}.");

            // Fetch the cart (e.g., from a repository or database)
            var cart = await _cartRepository.GetCartByIdAsync(cartId, cancellationToken);
            if (cart == null)
            {
                _logger.LogError($"Cart with ID {cartId} not found.");
                return false;
            }

            // Fetch the product (e.g., from a product repository)
            var product = await _productRepository.GetProductByIdAsync(Guid.Parse(productId), cancellationToken);
            if (product == null || product.StockQuantity < quantity)
            {
                _logger.LogError($"Product with ID {productId} not found or insufficient stock.");
                return false;
            }

            // Add the item to the cart
            cart.AddItem(productId, product.Name, product.Price, quantity);

            return true;
        }


        public async Task<bool> RemoveItemFromCartAsync(Guid cartId, string productId)
        {
            _logger.LogInformation($"Removing ProductId: {productId} from CartId: {cartId}.");
            var result = await _mediator.Send(new RemoveItemFromCartCommand(cartId, productId));
            if (!result.IsSuccess)
            {
                _logger.LogError($"Failed to remove item from cart: {result.Error}");
                return false;
            }
            return result.Value;
        }
        public async Task<bool> UpdateItemQuantityAsync(Guid cartId, string productId, int quantity)
        {
            _logger.LogInformation($"Updating quantity for ProductId: {productId} to {quantity} in CartId: {cartId}.");
            var result = await _mediator.Send(new UpdateCartItemQuantityCommand(cartId, productId, quantity));
            if (!result.IsSuccess)
            {
                _logger.LogError($"Failed to update item quantity: {result.Error}");
                return false;
            }
            return result.Value;
        }
        public async Task<bool> ClearCartAsync(Guid cartId)
        {
            _logger.LogInformation($"Clearing all items from CartId: {cartId}.");
            var result = await _mediator.Send(new ClearCartCommand(cartId));
            if (!result.IsSuccess)
            {
                _logger.LogError($"Failed to clear cart: {result.Error}");
                return false;
            }
            return result.Value;
        }
    }
}
