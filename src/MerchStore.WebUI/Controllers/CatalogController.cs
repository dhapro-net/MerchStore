using Microsoft.AspNetCore.Mvc;
using MediatR;
using MerchStore.Application.Catalog.Queries;
using MerchStore.WebUI.Models.Catalog;
using MerchStore.WebUI.Models;
using Microsoft.Extensions.Logging;

namespace MerchStore.WebUI.Controllers;

/// <summary>
/// Controller for managing the product catalog and shopping cart operations.
/// </summary>
public class CatalogController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<CatalogController> _logger;
    private readonly CookieShoppingCartService _cookieShoppingCartService;

    /// <summary>
    /// Constructor for CatalogController.
    /// </summary>
    /// <param name="mediator">Mediator for sending queries and commands.</param>
    /// <param name="logger">Logger for structured logging.</param>
    /// <param name="cookieShoppingCartService">Service for managing shopping cart cookies.</param>
    public CatalogController(IMediator mediator, ILogger<CatalogController> logger, CookieShoppingCartService cookieShoppingCartService)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cookieShoppingCartService = cookieShoppingCartService ?? throw new ArgumentNullException(nameof(cookieShoppingCartService));
    }

    /// <summary>
    /// Displays the product catalog.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        try
        {
            // Send the query to get all products
            var products = await _mediator.Send(new GetAllProductsQuery());

            // Map ProductDto to ProductCardViewModel
            var productViewModels = products.Select(p => new ProductCardViewModel
            {
                Id = p.Id,
                Name = p.Name,
                TruncatedDescription = p.Description.Length > 100
                    ? p.Description.Substring(0, 97) + "..."
                    : p.Description,
                FormattedPrice = p.Price.Amount.ToString("C"), // Format price as currency
                PriceAmount = p.Price.Amount,
                ImageUrl = p.ImageUrl?.ToString(),
                StockQuantity = p.StockQuantity,
            }).ToList();

            // Create the product catalog view model
            var viewModel = new ProductCatalogViewModel
            {
                FeaturedProducts = productViewModels
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            // Log the exception
            _logger.LogError(ex, "Error occurred while loading the product catalog.");

            // Show an error message to the user
            ViewBag.ErrorMessage = "An error occurred while loading products. Please try again later.";
            return View("Error");
        }
    }

    /// <summary>
    /// Displays the details of a specific product.
    /// </summary>
    /// <param name="id">The ID of the product.</param>
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            // Send the query to get product details
            var product = await _mediator.Send(new GetProductByIdQuery(id));

            // Map ProductDto to ProductDetailsViewModel
            var viewModel = new ProductDetailsViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                FormattedPrice = product.Price.Amount.ToString("C"), // Format price as currency
                PriceAmount = product.Price.Amount,
                ImageUrl = product.ImageUrl?.ToString(),
                StockQuantity = product.StockQuantity,
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            // Log the exception
            _logger.LogError(ex, "Error occurred while loading product details for ProductId: {ProductId}", id);

            // Show an error message to the user
            ViewBag.ErrorMessage = "An error occurred while loading the product. Please try again later.";
            return View("Error");
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

    private ErrorViewModel CreateErrorViewModel(string errorMessage)
    {
        return new ErrorViewModel
        {
            Message = errorMessage,
            RequestId = HttpContext.TraceIdentifier,
        };
    }
}