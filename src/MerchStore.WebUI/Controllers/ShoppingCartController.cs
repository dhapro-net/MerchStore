using Microsoft.AspNetCore.Mvc;
using MerchStore.Application.Services.Interfaces;
using MerchStore.WebUI.Models.ShoppingCart;
using MerchStore.Service.ShoppingCart;

public class ShoppingCartController : Controller
{
    private readonly IShoppingCartQueryService _shoppingCartQueryService;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly ILogger<ShoppingCartController> _logger;

    public ShoppingCartController(
        IShoppingCartQueryService shoppingCartQueryService,
        IShoppingCartService shoppingCartService,
        ILogger<ShoppingCartController> logger)
    {
        _shoppingCartQueryService = shoppingCartQueryService ?? throw new ArgumentNullException(nameof(shoppingCartQueryService));
        _shoppingCartService = shoppingCartService ?? throw new ArgumentNullException(nameof(shoppingCartService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var cardId = GetOrCreateCartId();
            var cartDto = await _shoppingCartQueryService.GetCartAsync(Guid.NewGuid());
            return View(cartDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Index");
            return View("Error", "An error occurred while loading the shopping cart.");
        }
    }
    [HttpPost]
    public IActionResult SubmitOrder(ShoppingCartViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Return the view with validation errors
            return View("Index", model);
        }

        // Process the shipping and payment information
        // Save the order, charge the payment, etc.

        return RedirectToAction("OrderCompleted");
    }

    public IActionResult OrderCompleted()
    {
        return View();
    }

    
    // Write operation using IShoppingCartService
    [HttpPost]
    public async Task<IActionResult> AddItemToCartAsync(string productId, int quantity)
    {
        try
        {
            var cartId = GetOrCreateCartId(); // Retrieve or create the cart ID
            await _shoppingCartService.AddItemToCartAsync(cartId, productId, quantity); // Add the item to the cart
            return RedirectToAction("Index"); // Redirect to the cart page
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AddItemToCartAsync");
            return View("Error", "An error occurred while adding the item to the cart.");
        }
    }
    private Guid GetOrCreateCartId()
    {
        var cartCookieKey = "ShoppingCartId";

        // Check if the cart ID exists in the request cookies
        if (Request.Cookies.TryGetValue(cartCookieKey, out var cartIdString) && Guid.TryParse(cartIdString, out var cartId))
        {
            return cartId;
        }

        // If not, generate a new cart ID and store it in a cookie
        cartId = Guid.NewGuid();
        Response.Cookies.Append(cartCookieKey, cartId.ToString(), new CookieOptions
        {
            Expires = DateTime.UtcNow.AddDays(7), // Set cookie expiration
            HttpOnly = true, // Prevent client-side scripts from accessing the cookie
            Secure = true, // Ensure the cookie is sent over HTTPS
            SameSite = SameSiteMode.Strict // Restrict cross-site cookie usage
        });

        return cartId;
    }
}