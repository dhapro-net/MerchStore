using Microsoft.AspNetCore.Mvc;
using MediatR;
using MerchStore.Application.Catalog.Queries;
using MerchStore.WebUI.Models.Catalog;
using MerchStore.WebUI.Models;

namespace MerchStore.WebUI.Controllers;

/// <summary>
/// Manages product catalog and shopping cart operations.
/// </summary>
public class CatalogController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<CatalogController> _logger;
    private readonly CookieShoppingCartService _cookieShoppingCartService;

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
            var products = await _mediator.Send(new GetAllProductsQuery());

            var productViewModels = products.Select(p => new ProductCardViewModel
            {
                Id = p.Id,
                Name = p.Name,
                TruncatedDescription = p.Description.Length > 100
                    ? p.Description.Substring(0, 97) + "..."
                    : p.Description,
                FormattedPrice = $"{p.Price.Amount:0.00} SEK",
                PriceAmount = p.Price.Amount,
                ImageUrl = p.ImageUrl?.ToString(),
                StockQuantity = p.StockQuantity,
            }).ToList();

            var viewModel = new ProductCatalogViewModel
            {
                FeaturedProducts = productViewModels
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading the product catalog.");
            ViewBag.ErrorMessage = "An error occurred while loading products. Please try again later.";
            return View("Error");
        }
    }

    /// <summary>
    /// Displays details of a specific product.
    /// </summary>
    /// <param name="id">The product ID.</param>
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var product = await _mediator.Send(new GetProductByIdQuery(id));

            var viewModel = new ProductDetailsViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                FormattedPrice = $"{product.Price.Amount:0.00} SEK",
                PriceAmount = product.Price.Amount,
                ImageUrl = product.ImageUrl?.ToString(),
                StockQuantity = product.StockQuantity,
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading product details for ProductId: {ProductId}", id);
            ViewBag.ErrorMessage = "An error occurred while loading the product. Please try again later.";
            return View("Error");
        }
    }

    /// <summary>
    /// Adds a product to the shopping cart.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="quantity">The quantity to add.</param>
    [HttpPost]
    public async Task<IActionResult> AddProductToCart(string productId, int quantity)
    {
        if (string.IsNullOrEmpty(productId))
        {
            TempData["ErrorMessage"] = "Product ID cannot be null or empty.";
            return RedirectToAction("Index");
        }

        if (quantity <= 0)
        {
            TempData["ErrorMessage"] = "Quantity must be greater than zero.";
            return RedirectToAction("Index");
        }

        try
        {
            var product = await _mediator.Send(new GetProductByIdQuery(Guid.Parse(productId)));

            if (product == null)
            {
                TempData["ErrorMessage"] = "The product could not be found.";
                return RedirectToAction("Index");
            }

            var cart = _cookieShoppingCartService.GetOrCreateCart();
            cart.AddProduct(productId, product.Name, product.Price, quantity);
            _cookieShoppingCartService.SaveCart(cart);

            TempData["SuccessMessage"] = "Product added to cart successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding product {ProductId} to the cart.", productId);
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
            _cookieShoppingCartService.ClearCart();
            TempData["SuccessMessage"] = "Shopping cart cleared successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing the shopping cart.");
            return View("Error", CreateErrorViewModel("An error occurred while clearing the shopping cart."));
        }
    }

    /// <summary>
    /// Creates an error view model with the specified message.
    /// </summary>
    private ErrorViewModel CreateErrorViewModel(string errorMessage)
    {
        return new ErrorViewModel
        {
            Message = errorMessage,
            RequestId = HttpContext.TraceIdentifier,
        };
    }
}