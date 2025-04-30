namespace MerchStore.Application.ShoppingCart.Commands
{
    public class RemoveItemFromCartCommand
    {
        public Guid CartId { get; set; }
        public string ProductId { get; set; }
    }
}