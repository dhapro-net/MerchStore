using Microsoft.AspNetCore.Mvc;
using MediatR;
using MerchStore.Application.ShoppingCart.Commands;
using MerchStore.Application.ShoppingCart.Queries;

public class ShoppingCartController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<ShoppingCartController> _logger;

    public ShoppingCartController(IMediator mediator, ILogger<ShoppingCartController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var cartId = GetOrCreateCartId();
            var cartDto = await _mediator.Send(new GetCartQuery(cartId));
            return View(cartDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Index");
            return View("Error", "An error occurred while loading the shopping cart.");
        }
    }

[HttpPost]
public async Task<IActionResult> AddItemToCartAsync(string productId, int quantity)
{
    try
    {
        var cartId = GetOrCreateCartId();
        // Use the constructor to instantiate the command
        var command = new AddItemToCartCommand(cartId, productId, quantity);
        await _mediator.Send(command);
        return RedirectToAction("Index");
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

        if (Request.Cookies.TryGetValue(cartCookieKey, out var cartIdString) && Guid.TryParse(cartIdString, out var cartId))
        {
            return cartId;
        }

        cartId = Guid.NewGuid();
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