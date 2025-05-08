namespace MerchStore.Application.ShoppingCart.Commands
{
    public class AddItemToCartCommand
    {
        public Guid CartId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}