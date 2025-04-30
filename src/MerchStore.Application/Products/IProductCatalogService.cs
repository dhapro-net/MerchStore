using MerchStore.Domain.Entities;

namespace MerchStore.Service.Products
{
    public interface IProductCatalogService
    {
        Task<Product> GetProductByIdAsync(string productId);
    }
}