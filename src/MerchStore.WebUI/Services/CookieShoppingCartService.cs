using MerchStore.Domain.ShoppingCart;
using System.Text.Json;
using Microsoft.Extensions.Logging;

public class CookieShoppingCartService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CookieShoppingCartService> _logger;
    private readonly ILogger<Cart> _cartLogger;
    private const string CartCookiePrefix = "ShoppingCart_";

    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    public CookieShoppingCartService(
        ILogger<CookieShoppingCartService> logger,
        ILogger<Cart> cartLogger,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cartLogger = cartLogger ?? throw new ArgumentNullException(nameof(cartLogger));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public Cart? GetCart(Guid cartId)
    {
        if (cartId == Guid.Empty)
        {
            _logger.LogWarning("Cart ID is Guid.Empty. Returning null.");
            return null;
        }

        if (_httpContextAccessor.HttpContext == null)
        {
            _logger.LogError("HttpContext is null. Unable to retrieve cart.");
            return null;
        }

        var cookieKey = $"{CartCookiePrefix}{cartId}";
        var cookieValue = _httpContextAccessor.HttpContext.Request.Cookies[cookieKey];

        if (string.IsNullOrEmpty(cookieValue))
        {
            _logger.LogWarning("Cart cookie with key '{CookieKey}' not found.", cookieKey);
            return null;
        }

        try
        {
            var cart = JsonSerializer.Deserialize<Cart>(cookieValue, _jsonSerializerOptions);
            _logger.LogInformation("Successfully retrieved cart with ID {CartId} from cookie.", cartId);
            return cart;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize cart with ID {CartId} from cookie.", cartId);
            return null;
        }
    }

    public void SaveCart(Cart cart)
    {
        if (cart == null)
        {
            _logger.LogError("Attempted to save a null cart. Operation aborted.");
            throw new ArgumentNullException(nameof(cart));
        }

        if (_httpContextAccessor.HttpContext == null)
        {
            _logger.LogError("HttpContext is null. Unable to save cart.");
            return;
        }

        var cookieKey = $"{CartCookiePrefix}{cart.CartId}";
        var serializedCart = JsonSerializer.Serialize(cart, _jsonSerializerOptions);

        _logger.LogInformation("Saving cart with ID {CartId} to cookie. Cookie key: {CookieKey}", cart.CartId, cookieKey);

        _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieKey, serializedCart, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(30)
        });

        _logger.LogInformation("Cart with ID {CartId} successfully saved to cookie.", cart.CartId);
    }

    public void ClearCart(Guid cartId)
    {
        if (cartId == Guid.Empty)
        {
            _logger.LogWarning("Cart ID is Guid.Empty. Cannot clear cart.");
            return;
        }

        if (_httpContextAccessor.HttpContext == null)
        {
            _logger.LogError("HttpContext is null. Unable to clear cart.");
            return;
        }

        var cookieKey = $"{CartCookiePrefix}{cartId}";
        _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieKey);

        _logger.LogInformation("Cart with ID {CartId} cleared from cookie.", cartId);
    }

    public Cart GetOrCreateCart(Guid cartId)
    {
        if (cartId == Guid.Empty)
        {
            _logger.LogWarning("Cart ID is Guid.Empty. Generating a new Cart ID.");
            cartId = Guid.NewGuid();
        }

        _logger.LogInformation("Retrieving or creating cart with ID: {CartId}.", cartId);

        var cart = GetCart(cartId);
        if (cart == null)
        {
            _logger.LogWarning("Cart with ID {CartId} not found. Creating a new cart.", cartId);

            cart = Cart.Create(cartId, _cartLogger);
            SaveCart(cart);
        }

        return cart;
    }
}