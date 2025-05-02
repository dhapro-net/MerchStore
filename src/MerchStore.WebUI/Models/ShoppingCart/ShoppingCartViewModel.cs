namespace MerchStore.WebUI.Models.ShoppingCart;

    public class ShoppingCartViewModel
    {
        public List<ShoppingCartItemViewModel> Items { get; set; } = new List<ShoppingCartItemViewModel>();
        public decimal TotalPrice { get; set; }
    }
