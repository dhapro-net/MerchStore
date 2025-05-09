using Microsoft.AspNetCore.Mvc;
using MediatR;
using MerchStore.Application.ShoppingCart.Commands;
using MerchStore.WebUI.Models.ShoppingCart;
using MerchStore.WebUI.Models;
using System.ComponentModel.DataAnnotations;
using MerchStore.Application.Catalog.Queries;

public class ShoppingCartController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<ShoppingCartController> _logger;
    private readonly CookieShoppingCartService _cookieShoppingCartService;

    public ShoppingCartController(CookieShoppingCartService cookieShoppingCartService, IMediator mediator, ILogger<ShoppingCartController> logger)
    {
        _cookieShoppingCartService = cookieShoppingCartService ?? throw new ArgumentNullException(nameof(cookieShoppingCartService));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Displays the shopping cart.
    /// </summary>
    public IActionResult Index()
    {
        try
        {
            // Retrieve or create the cart
            var cart = _cookieShoppingCartService.GetOrCreateCart(Guid.Empty);

            // Map Cart to ShoppingCartViewModel
            var viewModel = new ShoppingCartViewModel
            {
                CartId = cart.CartId,
                Products = cart.Products.Select(product => new ShoppingCartProductViewModel
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    UnitPrice = product.UnitPrice.Amount, // Map Money.Amount to decimal
                    Quantity = product.Quantity
                }).ToList(),
                TotalPrice = cart.CalculateTotal().Amount, // Calculate total price
                TotalProducts = cart.Products.Sum(p => p.Quantity),
                LastUpdated = cart.LastUpdated
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
    /// <param name="productId">The ID of the product to add.</param>
    /// <param name="quantity">The quantity of the product to add.</param>
    [HttpPost]
    public async Task<IActionResult> AddProductToCartAsync(string productId, int quantity)
    {
        try
        {
            // Retrieve product details using Mediator
            var product = await _mediator.Send(new GetProductByIdQuery(Guid.Parse(productId)));

            if (product == null)
            {
                TempData["ErrorMessage"] = "The product could not be found.";
                return RedirectToAction("Index");
            }

            // Retrieve or create the cart
            var cart = _cookieShoppingCartService.GetOrCreateCart(Guid.Empty);

            // Add the product to the cart
            cart.AddProduct(productId, product.Name, product.Price, quantity);

            // Save the updated cart to cookies
            _cookieShoppingCartService.SaveCart(cart);

            TempData["SuccessMessage"] = "Product added to cart successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding product {ProductId} to the cart.", productId);
            return View("Error", CreateErrorViewModel("An error occurred while adding the product to the cart."));
        }
    }

    /// <summary>
    /// Clears the shopping cart.
    /// </summary>
    [HttpPost]
    public IActionResult ClearCart()
    {
        try
        {
            // Clear the cart from cookies
            _cookieShoppingCartService.ClearCart(Guid.Empty);

            TempData["SuccessMessage"] = "Shopping cart cleared successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while clearing the shopping cart.");
            return View("Error", CreateErrorViewModel("An error occurred while clearing the shopping cart."));
        }
    }

    /// <summary>
    /// Creates an error view model with the specified message.
    /// </summary>
    private ErrorViewModel CreateErrorViewModel(string message)
    {
        return new ErrorViewModel
        {
            Message = message,
            RequestId = HttpContext.TraceIdentifier
        };
    }
}