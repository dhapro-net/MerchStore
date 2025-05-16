using MediatR;
using MerchStore.Application.ShoppingCart.Commands;
using MerchStore.Application.ShoppingCart.DTOs;
using MerchStore.Application.ShoppingCart.Interfaces;
using MerchStore.Domain.ShoppingCart;
using MerchStore.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
namespace MerchStore.Application.ShoppingCart.Handlers
{
public class CreateCartCommandHandler : IRequestHandler<CreateCartCommand, CartDto>
{
    private readonly ILogger<CreateCartCommandHandler> _logger;
    private readonly ILogger<Cart> _cartLogger;

    public CreateCartCommandHandler(
        ILogger<CreateCartCommandHandler> logger,
        ILogger<Cart> cartLogger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cartLogger = cartLogger ?? throw new ArgumentNullException(nameof(cartLogger));
    }

    public Task<CartDto> Handle(CreateCartCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating a new cart with ID: {CartId}.", request.CartId);

        // Create the cart
        var cart = Cart.Create(request.CartId, _cartLogger);

        // Map to CartDto
        return Task.FromResult(new CartDto
        {
            CartId = cart.CartId,
            Products = new List<CartProductDto>(),
            TotalPrice = new Money(0, "SEK"),
            TotalProducts = 0,
            LastUpdated = DateTime.UtcNow
        });
    }
}
}