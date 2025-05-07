using Microsoft.AspNetCore.Mvc;
using MerchStore.Application.Services.Interfaces;

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
            // Replace Guid.NewGuid() with the actual user/cart identifier
            var cartDto = await _shoppingCartQueryService.GetCartAsync(Guid.NewGuid());
            return View(cartDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Index");
            return View("Error", "An error occurred while loading the shopping cart.");
        }
    }

    // Read-only operation using IShoppingCartQueryService
    public async Task<IActionResult> GetCartAsync()
    {
        var cartDto = await _shoppingCartQueryService.GetCartAsync(Guid.NewGuid());
        return View(cartDto);
    }

    // Write operation using IShoppingCartService
    [HttpPost]
    public async Task<IActionResult> AddItemToCartAsync(Guid cartId, string productId, int quantity)
    {
        try
        {
            await _shoppingCartService.AddItemToCartAsync(cartId, productId, quantity);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AddItemToCartAsync");
            return View("Error", "An error occurred while adding the item to the cart.");
        }
    }
}