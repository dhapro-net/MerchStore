using MerchStore.Domain.ShoppingCart;
using System.Text.Json;

/// <summary>
/// Service for managing the shopping cart stored in browser cookies.
/// </summary>
public class CookieShoppingCartService
{
    private readonly IHttpContextAccessor _httpContextAccessor; // Provides access to the current HTTP context.
    private readonly ILogger<CookieShoppingCartService> _logger; // Logger for logging messages related to this service.
    private readonly ILogger<Cart> _cartLogger; // Logger for logging messages related to the Cart domain object.
    private const string CartCookieName = "ShoppingCart"; // The name of the cookie used to store the shopping cart.
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true, // Allows case-insensitive property matching during serialization/deserialization.
        WriteIndented = false // Keeps the serialized JSON compact.
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="CookieShoppingCartService"/> class.
    /// </summary>
    /// <param name="logger">Logger for this service.</param>
    /// <param name="cartLogger">Logger for the Cart domain object.</param>
    /// <param name="httpContextAccessor">Provides access to the current HTTP context.</param>
    public CookieShoppingCartService(
        ILogger<CookieShoppingCartService> logger,
        ILogger<Cart> cartLogger,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cartLogger = cartLogger ?? throw new ArgumentNullException(nameof(cartLogger));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    /// <summary>
    /// Retrieves the shopping cart from the browser cookie.
    /// </summary>
    /// <returns>The deserialized <see cref="Cart"/> object, or null if the cart does not exist or cannot be deserialized.</returns>
    public virtual Cart? GetCart()
    {
        if (_httpContextAccessor.HttpContext == null)
        {
            _logger.LogError("HttpContext is null. Unable to retrieve cart.");
            return null;
        }

        var cookieValue = _httpContextAccessor.HttpContext.Request.Cookies[CartCookieName];

        if (string.IsNullOrEmpty(cookieValue))
        {
            _logger.LogWarning("Cart cookie '{CartCookieName}' not found.", CartCookieName);
            return null;
        }

        try
        {
            var cart = JsonSerializer.Deserialize<Cart>(cookieValue, _jsonSerializerOptions);
            _logger.LogInformation("Successfully retrieved cart from cookie.");
            return cart;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize cart from cookie.");
            return null;
        }
    }

    /// <summary>
    /// Saves the shopping cart to the browser cookie.
    /// </summary>
    /// <param name="cart">The <see cref="Cart"/> object to save.</param>
    public virtual void SaveCart(Cart cart)
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

        var serializedCart = JsonSerializer.Serialize(cart, _jsonSerializerOptions);

        _logger.LogInformation("Saving cart to cookie with name '{CartCookieName}'.", CartCookieName);

        _httpContextAccessor.HttpContext.Response.Cookies.Append(CartCookieName, serializedCart, new CookieOptions
        {
            HttpOnly = true, // Prevents client-side scripts from accessing the cookie.
            Secure = true, // Ensures the cookie is only sent over HTTPS.
            SameSite = SameSiteMode.Lax, // Restricts cross-site cookie usage.
            Expires = DateTime.UtcNow.AddDays(30) // Sets the cookie to expire in 30 days.
        });

        _logger.LogInformation("Cart successfully saved to cookie.");
    }

    /// <summary>
    /// Clears the shopping cart by deleting the cookie.
    /// </summary>
    public virtual void ClearCart()
    {
        if (_httpContextAccessor.HttpContext == null)
        {
            _logger.LogError("HttpContext is null. Unable to clear cart.");
            return;
        }

        _httpContextAccessor.HttpContext.Response.Cookies.Delete(CartCookieName);

        _logger.LogInformation("Cart cleared from cookie.");
    }

    /// <summary>
    /// Retrieves the shopping cart from the cookie, or creates a new one if it does not exist.
    /// </summary>
    /// <returns>The <see cref="Cart"/> object.</returns>
    public virtual Cart GetOrCreateCart()
    {
        _logger.LogInformation("Retrieving or creating cart.");

        var cart = GetCart();
        if (cart == null)
        {
            _logger.LogWarning("Cart not found. Creating a new cart.");

            cart = Cart.Create(Guid.NewGuid(), _cartLogger); // Creates a new cart with a unique ID.
            SaveCart(cart); // Saves the newly created cart to the cookie.
        }

        return cart;
    }
}