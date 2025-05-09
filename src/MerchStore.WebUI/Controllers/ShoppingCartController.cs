using Microsoft.AspNetCore.Mvc;
using MediatR;
using MerchStore.Application.ShoppingCart.Commands;
using MerchStore.WebUI.Models.ShoppingCart;
using MerchStore.WebUI.Models;
using System.ComponentModel.DataAnnotations;
using MerchStore.Application.ShoppingCart.Interfaces;

public class ShoppingCartController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<ShoppingCartController> _logger;
    private readonly IShoppingCartQueryService _shoppingCartQueryService;


    public ShoppingCartController(IShoppingCartQueryService shoppingCartQueryService, IMediator mediator, ILogger<ShoppingCartController> logger)
    {
        _shoppingCartQueryService = shoppingCartQueryService ?? throw new ArgumentNullException(nameof(shoppingCartQueryService));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Displays the shopping cart.
    /// </summary>
    /// <summary>
    /// Displays the shopping cart.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        try
        {
            // Directly call the GetOrCreateCartAsync method from the ShoppingCartQueryService
            var cartDto = await _shoppingCartQueryService.GetOrCreateCartAsync(Guid.Empty, HttpContext.RequestAborted);

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
            // Log the error and return an error view
            _logger.LogError(ex, "Error occurred while loading the shopping cart.");
            return View("Error", CreateErrorViewModel("An error occurred while loading the shopping cart."));
        }
    }
    /// <summary>
    /// Adds a product to the shopping cart.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddProductToCartAsync(string productId, int quantity)
    {
        try
        {
            // Use the GetOrCreateCartAsync method to retrieve or create the cart
            var cartDto = await _shoppingCartQueryService.GetOrCreateCartAsync(Guid.Empty, HttpContext.RequestAborted);

            // Use Mediatr to send the AddProductToCartCommand
            var command = new AddProductToCartCommand(cartDto.CartId, productId, quantity, HttpContext.RequestAborted);
            await _mediator.Send(command);

            TempData["SuccessMessage"] = "Product added to cart successfully!";
            return RedirectToAction("Index");
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error occurred while adding product {ProductId} to the cart.", productId);
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding product {ProductId} to the cart.", productId);
            return View("Error", CreateErrorViewModel("An error occurred while adding the product to the cart."));
        }
    }



    /// <summary>
    /// Creates an error view model with the specified message.
    /// </summary>
    private ErrorViewModel CreateErrorViewModel(string message)
    {
        return new ErrorViewModel
        {
            Message = message
        };
    }
}