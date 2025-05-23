using Microsoft.AspNetCore.Mvc;
using MediatR;
using MerchStore.WebUI.Models.ShoppingCart;
using MerchStore.WebUI.Models;
using MerchStore.Application.Catalog.Queries;

public class ShoppingCartController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<ShoppingCartController> _logger;
    private readonly CookieShoppingCartService _cookieShoppingCartService;

    public ShoppingCartController(CookieShoppingCartService cookieShoppingCartService, IMediator mediator, ILogger<ShoppingCartController> logger)
    {
        _cookieShoppingCartService = cookieShoppingCartService ?? throw new ArgumentNullException(nameof(cookieShoppingCartService));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Displays the shopping cart.
    /// </summary>
    public IActionResult Index()
    {
        try
        {
            var cart = _cookieShoppingCartService.GetOrCreateCart();

            var viewModel = new ShoppingCartViewModel
            {
                CartId = cart.CartId,
                Products = cart.Products.Select(product => new ShoppingCartProductViewModel
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    UnitPrice = product.UnitPrice.Amount,
                    Quantity = product.Quantity
                }).ToList(),
                TotalPrice = cart.CalculateTotal().Amount,
                TotalProducts = cart.Products.Sum(p => p.Quantity),
                LastUpdated = cart.LastUpdated
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while loading the shopping cart.");
            return View("Error", CreateErrorViewModel("An error occurred while loading the shopping cart."));
        }
    }
    public IActionResult OrderCompleted()
    {
        return View("OrderCompleted");
    }
    /// <summary>
    /// Adds a product to the shopping cart.
    /// </summary>
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
            _cookieShoppingCartService.ClearCart();
            TempData["SuccessMessage"] = "Shopping cart cleared successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while clearing the shopping cart.");
            return View("Error", CreateErrorViewModel("An error occurred while clearing the shopping cart."));
        }
    }

    /// <summary>
    /// Updates the quantity of a product in the shopping cart.
    /// </summary>
    [HttpPost]
    public IActionResult UpdateQuantity(string productId, int quantity)
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
            var cart = _cookieShoppingCartService.GetOrCreateCart();
            cart.UpdateQuantity(productId, quantity);
            _cookieShoppingCartService.SaveCart(cart);

            TempData["SuccessMessage"] = "Product quantity updated successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating quantity for product {ProductId}.", productId);
            return View("Error", CreateErrorViewModel("An error occurred while updating the product quantity."));
        }
    }

    /// <summary>
    /// Removes a product from the shopping cart.
    /// </summary>
    [HttpPost]
    public IActionResult RemoveProduct(string productId)
    {
        if (string.IsNullOrEmpty(productId))
        {
            TempData["ErrorMessage"] = "Product ID cannot be null or empty.";
            return RedirectToAction("Index");
        }

        try
        {
            var cart = _cookieShoppingCartService.GetOrCreateCart();
            cart.RemoveProduct(productId);
            _cookieShoppingCartService.SaveCart(cart);

            TempData["SuccessMessage"] = "Product removed from cart successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while removing product {ProductId} from the cart.", productId);
            return View("Error", CreateErrorViewModel("An error occurred while removing the product from the cart."));
        }
    }

    /// <summary>
    /// Submits the shopping cart as an order.
    /// </summary>
    [HttpPost]
    public IActionResult SubmitOrder(ShoppingCartViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Please correct the errors in the form.";
            return RedirectToAction("Index");
        }

        try
        {
            var cart = _cookieShoppingCartService.GetCart();

            if (cart == null || !cart.Products.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty. Please add products before submitting an order.";
                return RedirectToAction("Index");
            }

            // Process the order (e.g., save to database, send confirmation email, etc.)
            _cookieShoppingCartService.ClearCart();

            TempData["SuccessMessage"] = "Order submitted successfully!";
            return RedirectToAction("OrderCompleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while submitting the order.");
            return View("Error", CreateErrorViewModel("An error occurred while submitting the order."));
        }
    }

    /// <summary>
    /// Creates an error view model with the specified message.
    /// </summary>
    private ErrorViewModel CreateErrorViewModel(string message)
    {
        return new ErrorViewModel
        {
            Message = message,
            RequestId = HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString()
        };
    }
}