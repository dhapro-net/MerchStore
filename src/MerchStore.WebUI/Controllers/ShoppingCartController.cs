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

    // Read-only operation using IShoppingCartQueryService
    public async Task<IActionResult> Index()
    {
        try
        {
            var viewModel = await _shoppingCartQueryService.GetCartAsync(Guid.NewGuid()); // Replace Guid.NewGuid() with the actual cart ID
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ShoppingCart Index");
            return View("Error", "An error occurred while loading the shopping cart.");
        }
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

    // Write operation using IShoppingCartService
    [HttpPost]
    public async Task<IActionResult> RemoveItemFromCartAsync(Guid cartId, string productId)
    {
        try
        {
            await _shoppingCartService.RemoveItemFromCartAsync(cartId, productId);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in RemoveItemFromCartAsync");
            return View("Error", "An error occurred while removing the item from the cart.");
        }
    }

    // Write operation using IShoppingCartService
    [HttpPost]
    public async Task<IActionResult> UpdateItemQuantityAsync(Guid cartId, string productId, int quantity)
    {
        try
        {
            await _shoppingCartService.UpdateItemQuantityAsync(cartId, productId, quantity);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UpdateItemQuantityAsync");
            return View("Error", "An error occurred while updating the item quantity.");
        }
    }
}