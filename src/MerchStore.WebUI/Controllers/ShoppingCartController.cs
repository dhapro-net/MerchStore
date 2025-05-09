using Microsoft.AspNetCore.Mvc;
using MediatR;
using MerchStore.Application.ShoppingCart.Commands;
using MerchStore.Application.ShoppingCart.Queries;
using MerchStore.WebUI.Models.ShoppingCart;
using MerchStore.Application.ShoppingCart.DTOs;
using MerchStore.Domain.ValueObjects;
using MerchStore.WebUI.Models;
using MerchStore.WebUI.Helpers;

public class ShoppingCartController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<ShoppingCartController> _logger;

    public ShoppingCartController(IMediator mediator, ILogger<ShoppingCartController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var cartId = GetOrCreateCartId();
            var cartDto = await _mediator.Send(new GetCartQuery(cartId));

            // Handle the case where cartDto is null
            if (cartDto == null)
            {
                cartDto = new CartDto
                {
                    CartId = cartId,
                    Products = new List<CartProductDto>(),
                    TotalPrice = new Money(0, "SEK"),
                    TotalProducts = 0,
                    LastUpdated = DateTime.UtcNow
                };
            }

            // Map CartDto to ShoppingCartViewModel
            var viewModel = new ShoppingCartViewModel
            {
                CartId = cartDto.CartId,
                Products = cartDto.Products?.Select(product => new ShoppingCartProductViewModel
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    UnitPrice = product.UnitPrice.Amount, // Map Money.Amount to decimal
                    Quantity = product.Quantity
                }).ToList() ?? new List<ShoppingCartProductViewModel>(), // Fallback to an empty list
                TotalPrice = cartDto.TotalPrice.Amount, // Map Money.Amount to decimal
                TotalProducts = cartDto.TotalProducts,
                LastUpdated = cartDto.LastUpdated
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Index");
            var errorViewModel = new ErrorViewModel
            {
                Message = "An error occurred while loading the shopping cart."
            };
            return View("Error", errorViewModel);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddProductToCartAsync(string productId, int quantity)
    {
        try
        {
            var cartId = GetOrCreateCartId();
            // Use the constructor to instantiate the command
            var command = new AddProductToCartCommand(cartId, productId, quantity);
            await _mediator.Send(command);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AddProductToCartAsync");
            var errorViewModel = new ErrorViewModel
            {
                Message = "An error occurred while adding the product to the cart."
            };
            return View("Error", errorViewModel);
        }
    }

    private Guid GetOrCreateCartId()
{
    return CartHelper.GetOrCreateCartId(HttpContext);
}
}