namespace MerchStore.WebUI.Models.ShoppingCart;

    public class ShoppingCartItemViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? FormattedPrice { get; set; }
        public decimal PriceAmount { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string? ImageUrl { get; set; }
    }
