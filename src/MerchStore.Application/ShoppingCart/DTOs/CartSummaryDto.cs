namespace MerchStore.Application.ShoppingCart.Dtos
{
    public class CartSummaryDto
    {
        public Guid CartId { get; set; }
        public int ItemsCount { get; set; }
        public decimal TotalPrice { get; set; }
    }
}