using Microsoft.AspNetCore.Http;

namespace MerchStore.WebUI.Helpers
{
    public static class CartHelper
    {
        private const string CartCookieKey = "ShoppingCartId";

        public static Guid GetOrCreateCartId(HttpContext httpContext)
        {
            if (httpContext.Request.Cookies.TryGetValue(CartCookieKey, out var cartIdString) && Guid.TryParse(cartIdString, out var cartId))
            {
                Console.WriteLine($"Retrieved CartId from cookie: {cartId}");
                return cartId;
            }

            cartId = Guid.NewGuid();
            Console.WriteLine($"Generated new CartId: {cartId}");
            httpContext.Response.Cookies.Append(CartCookieKey, cartId.ToString(), new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(7),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return cartId;
        }
    }
}