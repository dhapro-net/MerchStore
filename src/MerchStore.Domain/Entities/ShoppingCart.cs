namespace MerchStore.Domain.ShoppingCart
{
    public class ShoppingCart
    {
        public Guid CartId { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public decimal Total { get; set; }

        public void AddItem(string productId, object name, object price, int quantity)
        {
            throw new NotImplementedException();
        }

        public decimal CalculateTotal()
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void RemoveItem(string productId)
        {
            throw new NotImplementedException();
        }

        public void UpdateQuantity(string productId, int quantity)
        {
            throw new NotImplementedException();
        }
         public static ShoppingCart Create(Guid cartId)
        {
            return new ShoppingCart
            {
                CartId = cartId,
                Items = new List<CartItem>()
            };
        }
    }

    public class CartItem
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

    }
}