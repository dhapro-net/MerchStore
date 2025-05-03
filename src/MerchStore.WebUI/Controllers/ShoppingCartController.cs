public class ShoppingCartController : Controller
{
    private readonly IShoppingCartService _shoppingCartService;
    private readonly ILogger<ShoppingCartController> _logger;

    public ShoppingCartController(IShoppingCartService shoppingCartService, ILogger<ShoppingCartController> logger)
    {
        _shoppingCartService = shoppingCartService;
        _logger = logger;
    }

    public IActionResult Index()
    {
        try
        {
            var viewModel = _shoppingCartService.GetCartViewModel();
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ShoppingCart Index");
            return View("Error", "An error occurred while loading the shopping cart.");
        }
    }

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

    [HttpPost]
    public async IActionResult RemoveItemFromCartAsync(Guid cartId, string productId)
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
    
    [HttpPost]
    public async IActionResult UpdateItemQuantityAsync(Guid cartId, string productId,  int quantity)
    {   
        try
        {
            await _shoppingCartService.UpdateItemFromCartAsync(cartId, productId,  )
        }
    }
}