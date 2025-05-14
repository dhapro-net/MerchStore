using Microsoft.AspNetCore.Mvc;
using MediatR;
using MerchStore.Application.Catalog.Queries;
using MerchStore.WebUI.Models.Catalog;
using MerchStore.Application.ShoppingCart.Commands;
using Microsoft.AspNetCore.Authorization;
using MerchStore.Application.ShoppingCart.Interfaces;
using MerchStore.WebUI.Helpers;

namespace MerchStore.WebUI.Controllers;

public class CatalogController : Controller
{
    private readonly IMediator _mediator;
private readonly IShoppingCartService _shoppingCartService;

    public CatalogController(IMediator mediator, IShoppingCartService shoppingCartService)
{
    _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    _shoppingCartService = shoppingCartService ?? throw new ArgumentNullException(nameof(shoppingCartService));
}

    // GET: Catalog
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
            Console.WriteLine($"Error in ProductCatalog: {ex.Message}");

            // Show an error message to the user
            ViewBag.ErrorMessage = "An error occurred while loading products. Please try again later.";
            return View("Error");
        }
    }

    // GET: Store/Details/5
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
            Console.WriteLine($"Error in ProductDetails: {ex.Message}");

            // Show an error message to the user
            ViewBag.ErrorMessage = "An error occurred while loading the product. Please try again later.";
            return View("Error");
        }
    }
    [HttpPost]
    [HttpPost]
public async Task<IActionResult> AddProductToCart(Guid productId)
{
    try
    {
        if (productId == Guid.Empty)
        {
            Console.WriteLine("Invalid ProductId received.");
            TempData["ErrorMessage"] = "Invalid product ID.";
            return RedirectToAction("Index");
        }

        // Get or create the cart
        var cartId = GetOrCreateCartId();
        var cart = await _shoppingCartService.GetOrCreateCartAsync(cartId, HttpContext.RequestAborted);

        // Add the product to the cart
        var success = await _shoppingCartService.AddProductToCartAsync(cart.CartId, productId.ToString(), 1, HttpContext.RequestAborted);

        if (!success)
        {
            Console.WriteLine("Error adding product to cart.");
            TempData["ErrorMessage"] = "Failed to add product to cart.";
            return RedirectToAction("Index");
        }

        Console.WriteLine("Product added to cart successfully!");
        TempData["SuccessMessage"] = "Product added to cart successfully!";
        return RedirectToAction("Index");
    }
    catch (Exception ex)
    {
        // Log the exception
        Console.WriteLine($"Unexpected error in AddProductToCart: {ex.Message}");
        Console.WriteLine($"Stack Trace: {ex.StackTrace}");

        // Show a generic error message to the user
        TempData["ErrorMessage"] = "An unexpected error occurred while adding the product to the cart. Please try again later.";
        return RedirectToAction("Index", "ShoppingCart");
    }
}

private Guid GetOrCreateCartId()
{
    return CartHelper.GetOrCreateCartId(HttpContext);
}



}