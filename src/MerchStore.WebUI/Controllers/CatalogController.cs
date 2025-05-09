using Microsoft.AspNetCore.Mvc;
using MediatR;
using MerchStore.Application.Catalog.Queries;
using MerchStore.WebUI.Models.Catalog;
using MerchStore.Application.ShoppingCart.Commands;
using MerchStore.Application.ShoppingCart.Interfaces;
namespace MerchStore.WebUI.Controllers;

/// <summary>
/// Controller for managing the product catalog and shopping cart operations.
/// </summary>
public class CatalogController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<CatalogController> _logger;
    private readonly IShoppingCartQueryService _shoppingCartQueryService; // Add this field

    /// <summary>
    /// Constructor for CatalogController.
    /// </summary>
    /// <param name="mediator">Mediator for sending queries and commands.</param>
    /// <param name="logger">Logger for structured logging.</param>
    /// <param name="shoppingCartQueryService">Service for shopping cart queries.</param>
    public CatalogController(IMediator mediator, ILogger<CatalogController> logger, IShoppingCartQueryService shoppingCartQueryService)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _shoppingCartQueryService = shoppingCartQueryService ?? throw new ArgumentNullException(nameof(shoppingCartQueryService)); // Inject the service
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
    [HttpPost]
    public async Task<IActionResult> AddProductToCart(Guid productId)
    {
        if (productId == Guid.Empty)
        {
            _logger.LogWarning("Invalid ProductId received.");
            TempData["ErrorMessage"] = "Invalid product ID.";
            return RedirectToAction("Index");
        }

        try
        {
            // Retrieve the product details to validate stock
            var product = await _mediator.Send(new GetProductByIdQuery(productId));
            if (product == null || product.StockQuantity <= 0)
            {
                _logger.LogWarning("Product {ProductId} is out of stock or does not exist.", productId);
                TempData["ErrorMessage"] = "Product is out of stock or does not exist.";
                return RedirectToAction("Index");
            }

            // Retrieve or create the shopping cart using cookies
            var cartDto = await _shoppingCartQueryService.GetOrCreateCartAsync(Guid.Empty, HttpContext.RequestAborted);

            // Add the product to the cart
            var command = new AddProductToCartCommand(cartDto.CartId, productId.ToString(), 1, HttpContext.RequestAborted);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to add product {ProductId} to cart {CartId}.", productId, cartDto.CartId);
                TempData["ErrorMessage"] = "Failed to add product to cart.";
                return RedirectToAction("Index");
            }

            _logger.LogInformation("Product {ProductId} added to cart {CartId} successfully.", productId, cartDto.CartId);
            TempData["SuccessMessage"] = "Product added to cart successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while adding product {ProductId} to cart.", productId);
            TempData["ErrorMessage"] = "An unexpected error occurred while adding the product to the cart. Please try again later.";
            return RedirectToAction("Index");
        }
    }
}


