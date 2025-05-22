using MerchStore.Application.ShoppingCart.DTOs;
using MerchStore.Domain.ShoppingCart;
using MerchStore.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace MerchStore.Application.ShoppingCart.Mappers
{
    public static class CartMapper
    {
        public static CartDto CreateEmptyCartDto(Guid cartId)
        {
            if (cartId == Guid.Empty)
            {
                cartId = Guid.NewGuid();
            }

            return new CartDto
            {
                CartId = cartId,
                Products = new List<CartProductDto>(),
                TotalPrice = new Money(0, "SEK"),
                TotalProducts = 0,
                LastUpdated = DateTime.UtcNow
            };
        }

        public static CartDto ToCartDto(Cart cart)
        {
            

            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("CartMapper");
            logger.LogInformation("Mapping Cart to CartDto. Cart ID: {CartId}", cart.CartId);

            var products = cart.Products.Select(p => new CartProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Quantity = p.Quantity,
                UnitPrice = p.UnitPrice
            }).ToList();

            var totalPrice = new Money(
                products.Sum(p => p.UnitPrice.Amount * p.Quantity),
                products.FirstOrDefault()?.UnitPrice.Currency ?? "SEK"
            );

            return new CartDto
            {
                CartId = cart.CartId,
                Products = products,
                TotalPrice = totalPrice,
                TotalProducts = products.Sum(p => p.Quantity),
                LastUpdated = cart.LastUpdated
            };
        }


        public static Cart ToCart(CartDto cartDto, ILogger<Cart> logger)
        {
            if (cartDto == null)
            {
                logger.LogError("CartDto is null. Cannot map to Cart.");
                throw new ArgumentNullException(nameof(cartDto), "CartDto cannot be null.");
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
            }

            logger.LogInformation("Mapping CartDto to Cart. CartDto ID: {CartId}", cartDto.CartId);

            // Create a new Cart using the factory method
            var cart = Cart.Create(cartDto.CartId, logger);

            // Populate the cart's products
            foreach (var productDto in cartDto.Products)
            {
                logger.LogInformation("Adding product to Cart. Product ID: {ProductId}, Quantity: {Quantity}", productDto.ProductId, productDto.Quantity);

                cart.AddProduct(
                    productId: productDto.ProductId,
                    name: productDto.ProductName,
                    price: productDto.UnitPrice,
                    quantity: productDto.Quantity
                );
            }

            logger.LogInformation("Successfully mapped CartDto to Cart. Cart ID: {CartId}", cart.CartId);

            return cart;
        }
    }
}