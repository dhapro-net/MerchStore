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

        public List<CartItem> Items { get; private set; } = new List<CartItem>();
        public DateTime CreatedAt { get; private set; }
        public DateTime LastUpdated { get; private set; }

        // Parameterless constructor for deserialization
        private Cart()
        {
        }

        public Cart(Guid cartId, List<CartItem> items, DateTime createdAt, DateTime lastUpdated)
        {
            CartId = cartId;
            Items = items ?? new List<CartItem>();
            CreatedAt = createdAt;
            LastUpdated = lastUpdated;
        }

        public void AddItem(string productId, string name, Money price, int quantity)
        {
            if (string.IsNullOrEmpty(productId))
                throw new ArgumentException("Product ID cannot be null or empty", nameof(productId));

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            var existingItem = Items.FirstOrDefault(i => i.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                var newItem = new CartItem(
                    productId,
                    name,
                    price,
                    quantity
                );

                Items.Add(newItem);
            }

            UpdateLastModified();

            // Add domain event
            AddDomainEvent(new CartItemAddedEvent(CartId, productId, name, price, quantity));
        }

        public Money CalculateTotal()
        {
            if (!Items.Any())
                return new Money(0, "SEK"); // Default to 0 with a default currency

            var totalAmount = Items
                .Select(item => item.UnitPrice * item.Quantity) // Multiply Money by quantity
                .Aggregate((sum, next) => sum + next); // Sum up Money objects

            return totalAmount;
        }

        public void Clear()
        {
            Items.Clear();
            UpdateLastModified();

            // Add domain event
            AddDomainEvent(new CartClearedEvent(CartId));
        }

        public void RemoveItem(string productId)
        {
            if (string.IsNullOrEmpty(productId))
                throw new ArgumentException("Product ID cannot be null or empty", nameof(productId));

            var itemToRemove = Items.FirstOrDefault(i => i.ProductId == productId);

            if (itemToRemove != null)
            {
                Items.Remove(itemToRemove);
                UpdateLastModified();

                // Add domain event
                AddDomainEvent(new CartItemRemovedEvent(CartId, productId));
            }
        }

        public void UpdateQuantity(string productId, int quantity)
        {
            if (string.IsNullOrEmpty(productId))
                throw new ArgumentException("Product ID cannot be null or empty", nameof(productId));

            if (quantity <= 0)
            {
                RemoveItem(productId);
                return;
            }

            var item = Items.FirstOrDefault(i => i.ProductId == productId);

            if (item != null)
            {
                item.UpdateQuantity(quantity);
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
            var cart = new Cart(cartId, new List<CartItem>(), DateTime.UtcNow, DateTime.UtcNow);
            logger.LogInformation($"Cart created with ID: {cartId}");

            return cart;
        }

        // Domain methods to query cart state
        public bool HasItems() => Items.Any();

        public int ItemCount() => Items.Sum(i => i.Quantity);

        public bool ContainsProduct(string productId) =>
            Items.Any(i => i.ProductId == productId);

        public CartItem GetItem(string productId) =>
            Items.FirstOrDefault(i => i.ProductId == productId);
    }
}