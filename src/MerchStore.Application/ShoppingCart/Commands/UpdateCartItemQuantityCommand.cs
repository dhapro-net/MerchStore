namespace MerchStore.Application.ShoppingCart.Commands
{
    public class UpdateCartItemQuantityCommand
    {
        public Guid CartId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}