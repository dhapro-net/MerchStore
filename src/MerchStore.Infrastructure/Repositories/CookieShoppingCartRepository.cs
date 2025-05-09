using Microsoft.AspNetCore.Http;
using MerchStore.Domain.ShoppingCart;
using MerchStore.Domain.ShoppingCart.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace MerchStore.Infrastructure.Repositories
{
    public class CookieShoppingCartRepository : IShoppingCartCommandRepository, IShoppingCartQueryRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CartCookiePrefix = "ShoppingCartId";
        private readonly CookieOptions _cookieOptions;
        private readonly ILogger<CookieShoppingCartRepository> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };

        public CookieShoppingCartRepository(IHttpContextAccessor httpContextAccessor, ILogger<CookieShoppingCartRepository> logger)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _cookieOptions = new CookieOptions
            {
                HttpOnly = false,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(30),
                IsEssential = true // For GDPR compliance
            };
        }

        public async Task<Cart?> GetCartByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Check for Guid.Empty
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
                _logger.LogInformation($"Generated cookie key: {cookieKey}");

                var cookieValue = _httpContextAccessor.HttpContext.Request.Cookies[cookieKey];

                if (string.IsNullOrEmpty(cookieValue))
                {
                    _logger.LogWarning($"Cart cookie with key '{cookieKey}' not found.");
                    return null;
                }

                var cart = JsonSerializer.Deserialize<Cart>(cookieValue, _jsonSerializerOptions);
                if (cart == null)
                {
                    _logger.LogWarning($"Deserialized cart is null for cookie key '{cookieKey}'.");
                    return null;
                }

                _logger.LogInformation($"Successfully retrieved cart with ID {id}.");
                return cart;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Operation was canceled while retrieving cart with ID {CartId}.", id);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deserializing cart with ID {id}: {ex.Message}");
                return null;
            }
        }

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

        public Task UpdateAsync(Cart cart)
        {
            _logger.LogInformation("Updating cart with ID {CartId} in cookies.", cart.CartId);
            return AddAsync(cart, CancellationToken.None);
        }

        public Task DeleteAsync(Guid id)
        {
            var cookieKey = GetCookieKeyForCart(id);
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete(cookieKey);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(Guid id)
        {
            var cookieKey = GetCookieKeyForCart(id);
            var exists = _httpContextAccessor.HttpContext?.Request.Cookies.ContainsKey(cookieKey) ?? false;
            return Task.FromResult(exists);
        }

        private string GetCookieKeyForCart(Guid id) => $"{CartCookiePrefix}{id}";
    }
}