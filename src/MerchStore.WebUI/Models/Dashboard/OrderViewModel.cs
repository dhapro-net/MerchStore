

namespace MerchStore.WebUI.Models.Dashboard;


public class OrderViewModel
    {
        public Guid Id { get; set; }
        public required string CustomerName { get; set; }
        public required string CustomerEmail { get; set; }
        public required string Status { get; set; }
        public int ItemCount { get; set; }
        public decimal Total { get; set; }
        public string Currency { get; set; } = "SEK";
    }