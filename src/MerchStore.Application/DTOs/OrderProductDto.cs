namespace MerchStore.Application.DTOs;




public class OrderProductDto
{
    public required string CustomerName { get; set; }
    public required string CustomerEmail { get; set; } = "";
    public required string StreetAddress { get; set; }
    public required string City { get; set; }
    public required string ZipCode { get; set; }
    public required string Country { get; set; }

}