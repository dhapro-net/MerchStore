using Microsoft.AspNetCore.Http;
using MerchStore.Domain.ShoppingCart;
using MerchStore.Domain.ShoppingCart.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace MerchStore.Infrastructure.Repositories;

/// <summary>
/// Repository for managing shopping cart data stored in browser cookies.
/// Implements both command and query repository interfaces.
/// </summary>
public class CookieShoppingCartRepository : IShoppingCartCommandRepository, IShoppingCartQueryRepository
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly CookieOptions _cookieOptions;
    private readonly ILogger<CookieShoppingCartRepository> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };
    private const string CartCookieName = "ShoppingCart";

    /// <summary>
    /// Initializes a new instance of the <see cref="CookieShoppingCartRepository"/> class.
    /// </summary>
    public CookieShoppingCartRepository(IHttpContextAccessor httpContextAccessor, ILogger<CookieShoppingCartRepository> logger)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _cookieOptions = new CookieOptions
        {
            HttpOnly = true, // Prevents client-side scripts from accessing the cookie.
            Secure = true, // Ensures the cookie is only sent over HTTPS.
            SameSite = SameSiteMode.Lax, // Restricts cross-site cookie usage.
            Expires = DateTime.UtcNow.AddDays(30), // Sets the cookie to expire in 30 days.
            IsEssential = true // Ensures compliance with GDPR by marking the cookie as essential.
        };
    }

    /// <summary>
    /// Retrieves a shopping cart by its ID from cookies.
    /// </summary>
    public async Task<Cart?> GetCartByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            _logger.LogWarning("Attempted to retrieve a cart with Guid.Empty. Returning null.");
            return null;
        }

        if (_httpContextAccessor.HttpContext == null)
        {
            _logger.LogWarning("HttpContext is null. Unable to retrieve cart.");
            return null;
        }

        var cookieKey = GetCookieKeyForCart(id);
        var cookieValue = _httpContextAccessor.HttpContext.Request.Cookies[cookieKey];

        if (string.IsNullOrEmpty(cookieValue))
        {
            _logger.LogWarning($"Cart cookie with key '{cookieKey}' not found.");
            return null;
        }

        try
        {
            var cart = JsonSerializer.Deserialize<Cart>(cookieValue, _jsonSerializerOptions);
            if (cart == null)
            {
                _logger.LogWarning($"Deserialized cart is null for cookie key '{cookieKey}'.");
                return null;
            }

            _logger.LogInformation($"Successfully retrieved cart with ID {id}.");
            return cart;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deserializing cart with ID {CartId}.", id);
            return null;
        }
    }

    /// <summary>
    /// Adds a new shopping cart to cookies.
    /// </summary>
    public Task AddAsync(Cart cart, CancellationToken cancellationToken)
    {
        if (cart == null)
            throw new ArgumentNullException(nameof(cart));

        var cookieKey = GetCookieKeyForCart(cart.CartId);
        var serializedCart = JsonSerializer.Serialize(cart, _jsonSerializerOptions);

        _httpContextAccessor.HttpContext?.Response.Cookies.Append(
            cookieKey,
            serializedCart,
            _cookieOptions);

        _logger.LogInformation("Cart with ID {CartId} added to cookies.", cart.CartId);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Updates an existing shopping cart in cookies.
    /// </summary>
    public Task UpdateAsync(Cart cart)
    {
        _logger.LogInformation("Updating cart with ID {CartId} in cookies.", cart.CartId);
        return AddAsync(cart, CancellationToken.None);
    }

    /// <summary>
    /// Deletes a shopping cart from cookies by its ID.
    /// </summary>
    public Task DeleteAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            _logger.LogWarning("Attempted to delete a cart with Guid.Empty. Operation skipped.");
            return Task.CompletedTask;
        }

        var cookieKey = GetCookieKeyForCart(id);
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete(cookieKey);

        _logger.LogInformation("Cart with ID {CartId} deleted from cookies.", id);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Checks if a shopping cart exists in cookies by its ID.
    /// </summary>
    public Task<bool> ExistsAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            _logger.LogWarning("Attempted to check existence of a cart with Guid.Empty. Returning false.");
            return Task.FromResult(false);
        }

        var cookieKey = GetCookieKeyForCart(id);
        var exists = _httpContextAccessor.HttpContext?.Request.Cookies.ContainsKey(cookieKey) ?? false;

        _logger.LogInformation("Cart with ID {CartId} exists: {Exists}.", id, exists);
        return Task.FromResult(exists);
    }

    /// <summary>
    /// Generates a unique cookie key for a shopping cart based on its ID.
    /// </summary>
    private string GetCookieKeyForCart(Guid id) => $"{CartCookieName}{id}";
}