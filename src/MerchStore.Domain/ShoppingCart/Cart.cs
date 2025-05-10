using System;
using System.Collections.Generic;
using System.Linq;
using MerchStore.Domain.Common;
using MerchStore.Domain.ValueObjects;
using MerchStore.Domain.ShoppingCart.Events;
using Microsoft.Extensions.Logging;

namespace MerchStore.Domain.ShoppingCart
{
    public class Cart : AggregateRoot<Guid>
    {
        private readonly ILogger<Cart> _logger;

        public Guid CartId
        {
            get => Id;
            private set => Id = value;
        }

        public List<CartProduct> Products { get; private set; } = new List<CartProduct>();
        public DateTime CreatedAt { get; private set; }
        public DateTime LastUpdated { get; private set; }

        // Parameterless constructor for deserialization
        private Cart()
        {
        }

        public Cart(Guid cartId, List<CartProduct> products, DateTime createdAt, DateTime lastUpdated)
        {
            CartId = cartId;
            Products = products ?? new List<CartProduct>();
            CreatedAt = createdAt;
            LastUpdated = lastUpdated;
        }

        public void AddProduct(string productId, string name, Money price, int quantity)
        {
            if (string.IsNullOrEmpty(productId))
                throw new ArgumentException("Product ID cannot be null or empty", nameof(productId));

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            var existingProduct = Products.FirstOrDefault(i => i.ProductId == productId);

            if (existingProduct != null)
            {
                existingProduct.UpdateQuantity(existingProduct.Quantity + quantity);
            }
            else
            {
                var newProduct = new CartProduct(
                    productId,
                    name,
                    price,
                    quantity
                );

                Products.Add(newProduct);
            }

            UpdateLastModified();

            // Add domain event
            AddDomainEvent(new CartProductAddedEvent(CartId, productId, name, price, quantity));
        }

        public Money CalculateTotal()
        {
            if (!Products.Any())
                return new Money(0, "SEK"); // Default to 0 with a default currency

            var totalAmount = Products
                .Select(product => product.UnitPrice * product.Quantity) // Multiply Money by quantity
                .Aggregate((sum, next) => sum + next); // Sum up Money objects

            return totalAmount;
        }

        public void Clear()
        {
            Products.Clear();
            UpdateLastModified();

            // Add domain event
            AddDomainEvent(new CartClearedEvent(CartId));
        }

        public void RemoveProduct(string productId)
        {
            if (string.IsNullOrEmpty(productId))
                throw new ArgumentException("Product ID cannot be null or empty", nameof(productId));

            var productToRemove = Products.FirstOrDefault(i => i.ProductId == productId);

            if (productToRemove != null)
            {
                Products.Remove(productToRemove);
                UpdateLastModified();

                // Add domain event
                AddDomainEvent(new CartProductRemovedEvent(CartId, productId));
            }
        }

        public void UpdateQuantity(string productId, int quantity)
        {
            if (string.IsNullOrEmpty(productId))
                throw new ArgumentException("Product ID cannot be null or empty", nameof(productId));

            if (quantity <= 0)
            {
                RemoveProduct(productId);
                return;
            }

            var product = Products.FirstOrDefault(i => i.ProductId == productId);

            if (product != null)
            {
                product.UpdateQuantity(quantity);
                UpdateLastModified();
            }
        }

        private void UpdateLastModified()
        {
            LastUpdated = DateTime.UtcNow;
        }

        public static Cart Create(Guid cartId, ILogger<Cart> logger)
        {
            if (cartId == Guid.Empty)
                throw new ArgumentException("Cart ID cannot be empty", nameof(cartId));

            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            // Create a new cart with default values
            var cart = new Cart(cartId, new List<CartProduct>(), DateTime.UtcNow, DateTime.UtcNow);
            logger.LogInformation($"Cart created with ID: {cartId}");

            return cart;
        }

        // Domain methods to query cart state
        public bool HasProducts() => Products.Any();

        public int ProductCount() => Products.Sum(i => i.Quantity);

        public bool ContainsProduct(string productId) =>
            Products.Any(i => i.ProductId == productId);

        public CartProduct GetProduct(string productId) =>
            Products.FirstOrDefault(i => i.ProductId == productId);
    }
}