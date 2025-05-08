using Microsoft.AspNetCore.Mvc;
using MediatR;
using MerchStore.Application.Catalog.Queries;
using MerchStore.WebUI.Models.Catalog;
using MerchStore.Application.ShoppingCart.Commands;
using Microsoft.AspNetCore.Authorization;

namespace MerchStore.WebUI.Controllers;

public class CatalogController : Controller
{
    private readonly IMediator _mediator;

    public CatalogController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
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
    public async Task<IActionResult> AddToCart(Guid productId)
    {
    try
    {
        if (productId == Guid.Empty)
        {
            Console.WriteLine("Invalid ProductId received.");
            TempData["ErrorMessage"] = "Invalid product ID.";
            return RedirectToAction("Index");
        }

        var cartId = GetOrCreateCartId();
        Console.WriteLine($"Using CartId: {cartId}");

        var result = await _mediator.Send(new AddItemToCartCommand(cartId, productId.ToString(), 1));

        if (!result.IsSuccess)
        {
            Console.WriteLine($"Error adding item to cart: {result.Error}");
            TempData["ErrorMessage"] = result.Error;
            return RedirectToAction("Index");
        }

        Console.WriteLine("Item added to cart successfully!");
        TempData["SuccessMessage"] = "Item added to cart successfully!";
        return RedirectToAction("Index");
    }
    catch (Exception ex)
    {
        // Log the exception
        Console.WriteLine($"Unexpected error in AddToCart: {ex.Message}");
        Console.WriteLine($"Stack Trace: {ex.StackTrace}");

        // Show a generic error message to the user
        TempData["ErrorMessage"] = "An unexpected error occurred while adding the item to the cart. Please try again later.";
        return RedirectToAction("Index");
    }
}
    private Guid GetOrCreateCartId()
    {
        const string cartCookieKey = "ShoppingCartId";

        if (Request.Cookies.TryGetValue(cartCookieKey, out var cartIdString) && Guid.TryParse(cartIdString, out var cartId))
        {
            Console.WriteLine($"Retrieved CartId from cookie: {cartId}");
            return cartId;
        }

        cartId = Guid.NewGuid();
        Console.WriteLine($"Generated new CartId: {cartId}");
        Response.Cookies.Append(cartCookieKey, cartId.ToString(), new CookieOptions
        {
            Expires = DateTime.UtcNow.AddDays(7),
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });

        return cartId;
    }

}