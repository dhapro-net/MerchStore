using MediatR;
using MerchStore.Application.ShoppingCart.Commands;
using MerchStore.Application.ShoppingCart.DTOs;
using MerchStore.Domain.ShoppingCart;
using MerchStore.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

public class CreateCartCommandHandler : IRequestHandler<CreateCartCommand, CartDto>
{
    private readonly ILogger<CreateCartCommandHandler> _logger;

    public CreateCartCommandHandler(ILogger<CreateCartCommandHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<CartDto> Handle(CreateCartCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating a new cart with ID: {CartId}.", request.CartId);

        // Create the cart
        var cart = Cart.Create(request.CartId, _logger as ILogger<Cart>);

        // Map to CartDto
        return new CartDto
        {
            CartId = cart.CartId,
            Products = new List<CartProductDto>(),
            TotalPrice = new Money(0, "SEK"),
            TotalProducts = 0,
            LastUpdated = DateTime.UtcNow
        };
    }
}