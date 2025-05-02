using Microsoft.AspNetCore.Mvc;
using MerchStore.Application.Services.Interfaces;
using MerchStore.WebUI.Models.ShoppingCart;
using System.Text.Json;

namespace MerchStore.WebUI.Controllers;

public class ShoppingCartController : Controller
{
    private const string CartCookieName = "ShoppingCart";
    private readonly ICatalogService _catalogService;

    public ShoppingCartController(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    // GET: ShoppingCart
    public IActionResult Index()
    {
        try
        {
            // Retrieve the cart from the cookie
            var cart = GetCartFromCookie();

            // Map to the view model
            var viewModel = new ShoppingCartViewModel
            {
                Items = cart.Select(item => new ShoppingCartItemViewModel
                {
                    Id = item.ProductId,
                    Name = item.Name,
                    FormattedPrice = item.PriceAmount.ToString("C"), // Format price as currency
                    PriceAmount = item.PriceAmount,
                    Quantity = item.Quantity,
                    TotalPrice = item.PriceAmount * item.Quantity, // Calculate total price for each item
                    ImageUrl = item.ImageUrl
                }).ToList(),
                TotalPrice = cart.Sum(item => item.PriceAmount * item.Quantity) // Calculate total price for the cart
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in ShoppingCart Index: {ex.Message}");
            ViewBag.ErrorMessage = "An error occurred while loading the shopping cart. Please try again later.";
            return View("Error");
        }
    }

    // POST: ShoppingCart/Add
    [HttpPost]
    public async Task<IActionResult> AddToCart(Guid productId, int quantity)
    {
        try
        {
            // Retrieve the product from the catalog service
            var product = await _catalogService.GetProductByIdAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            // Retrieve the cart from the cookie
            var cart = GetCartFromCookie();

            // Check if the item already exists in the cart
            var existingItem = cart.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new ShoppingCartItem
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    PriceAmount = product.Price.Amount,
                    Quantity = quantity,
                    ImageUrl = product.ImageUrl?.ToString()
                });
            }

            // Save the updated cart back to the cookie
            SaveCartToCookie(cart);

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in AddToCart: {ex.Message}");
            ViewBag.ErrorMessage = "An error occurred while adding the item to the cart. Please try again later.";
            return View("Error");
        }
    }

    // POST: ShoppingCart/Remove
    [HttpPost]
    public IActionResult RemoveFromCart(Guid productId)
    {
        try
        {
            // Retrieve the cart from the cookie
            var cart = GetCartFromCookie();

            // Remove the item
            cart.RemoveAll(i => i.ProductId == productId);

            // Save the updated cart back to the cookie
            SaveCartToCookie(cart);

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in RemoveFromCart: {ex.Message}");
            ViewBag.ErrorMessage = "An error occurred while removing the item from the cart. Please try again later.";
            return View("Error");
        }
    }

    private List<ShoppingCartItem> GetCartFromCookie()
    {
        var cookieValue = Request.Cookies[CartCookieName];
        if (string.IsNullOrEmpty(cookieValue))
        {
            return new List<ShoppingCartItem>();
        }

        return JsonSerializer.Deserialize<List<ShoppingCartItem>>(cookieValue) ?? new List<ShoppingCartItem>();
    }

    private void SaveCartToCookie(List<ShoppingCartItem> cart)
    {
        var cookieValue = JsonSerializer.Serialize(cart);
        Response.Cookies.Append(CartCookieName, cookieValue, new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTimeOffset.UtcNow.AddDays(7) // Cookie expiration
        });
    }
}

