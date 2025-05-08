using MediatR;
using MerchStore.Application.ShoppingCart.Commands;
using MerchStore.Application.ShoppingCart.DTOs;
using MerchStore.Application.ShoppingCart.Interfaces;
using Microsoft.Extensions.Logging;

namespace MerchStore.Application.ShoppingCart.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ShoppingCartService> _logger;

        public ShoppingCartService(IMediator mediator, ILogger<ShoppingCartService> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CartDto> GetCartAsync(Guid cartId)
        {
            _logger.LogInformation($"Fetching cart with ID {cartId}.");
            return await _mediator.Send(new GetCartQuery(cartId));
        }

        public async Task<bool> AddItemToCartAsync(Guid cartId, string productId, int quantity)
        {
            _logger.LogInformation($"Adding ProductId: {productId} with Quantity: {quantity} to CartId: {cartId}.");
            return await _mediator.Send(new AddItemToCartCommand(cartId, productId, quantity));
        }

        public async Task<bool> RemoveItemFromCartAsync(Guid cartId, string productId)
        {
            _logger.LogInformation($"Removing ProductId: {productId} from CartId: {cartId}.");
            return await _mediator.Send(new RemoveItemFromCartCommand(cartId, productId));
        }

        public async Task<bool> UpdateItemQuantityAsync(Guid cartId, string productId, int quantity)
        {
            _logger.LogInformation($"Updating quantity for ProductId: {productId} to {quantity} in CartId: {cartId}.");
            return await _mediator.Send(new UpdateCartItemQuantityCommand(cartId, productId, quantity));
        }

        public async Task<bool> ClearCartAsync(Guid cartId)
        {
            _logger.LogInformation($"Clearing all items from CartId: {cartId}.");
            return await _mediator.Send(new ClearCartCommand(cartId));
        }

        public async Task<decimal> CalculateCartTotalAsync(Guid cartId)
        {
            _logger.LogInformation($"Calculating total for CartId: {cartId}.");
            var cart = await GetCartAsync(cartId);
            return cart.TotalPrice;
        }
    }
}