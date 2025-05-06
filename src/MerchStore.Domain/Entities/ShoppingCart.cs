namespace MerchStore.Domain.ShoppingCart
{
    public class ShoppingCart
    {
        public Guid CartId { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime LastUpdated { get; private set; }

        public void AddItem(string productId, object name, object price, int quantity)
        {
            if (string.IsNullOrEmpty(productId))
                throw new ArgumentException("Product ID cannot be null or empty", nameof(productId));
            
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            var existingItem = Items.FirstOrDefault(i => i.ProductId == productId);
            
            if (existingItem != null)
            {
                // Update existing item quantity
                existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                // Add new item
                var newItem = new CartItem(
                    productId,
                    (string)name,
                    (decimal)price,
                    quantity
                );
                
                Items.Add(newItem);
            }
            
            UpdateLastModified();
        }

        public decimal CalculateTotal()
        {
            return Items.Sum(item => item.UnitPrice * item.Quantity);
        }

        public void Clear()
        {
            Items.Clear();
            UpdateLastModified();
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
        
        public static ShoppingCart Create(Guid cartId)
        {
            if (cartId == Guid.Empty)
                throw new ArgumentException("Cart ID cannot be empty", nameof(cartId));
                
            return new ShoppingCart
            {
                CartId = cartId,
                Items = new List<CartItem>(),
                CreatedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };
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