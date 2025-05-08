using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MerchStore.Domain.ShoppingCart;
using MerchStore.Domain.ShoppingCart.Interfaces;

namespace MerchStore.Infrastructure.Repositories
{
    public class CookieShoppingCartRepository : IShoppingCartRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CartCookiePrefix = "MerchStore_Cart_";
        private readonly CookieOptions _cookieOptions;
        
        public CookieShoppingCartRepository(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            
            _cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(30),
                IsEssential = true // For GDPR compliance
            };
        }
        
        public Task<Cart> GetCartByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            // Check if the operation has been canceled
            cancellationToken.ThrowIfCancellationRequested();

            var cookieKey = GetCookieKeyForCart(id);
            var cookieValue = _httpContextAccessor.HttpContext?.Request.Cookies[cookieKey];

            if (string.IsNullOrEmpty(cookieValue))
                return Task.FromResult<Cart>(null);

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var cart = JsonSerializer.Deserialize<Cart>(cookieValue, options);
                return Task.FromResult(cart);
            }
            catch (Exception)
            {
                return Task.FromResult<Cart>(null);
            }
        }
        
        public Task AddAsync(Cart cart)
        {
            if (cart == null) 
                throw new ArgumentNullException(nameof(cart));
                
            var cookieKey = GetCookieKeyForCart(cart.CartId);
            var options = new JsonSerializerOptions { WriteIndented = false };
            var serializedCart = JsonSerializer.Serialize(cart, options);
            
            _httpContextAccessor.HttpContext?.Response.Cookies.Append(
                cookieKey,
                serializedCart,
                _cookieOptions);
                
            return Task.CompletedTask;
        }
        
        public Task UpdateAsync(Cart cart)
        {
            return AddAsync(cart); // Same implementation for cookies
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