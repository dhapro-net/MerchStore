using MerchStore.Domain.Entities;

namespace MerchStore.Application.Services.Interfaces;


public interface ICatalogService
{

    Task<IEnumerable<Product>> GetAllProductsAsync();


    Task<Product?> GetProductByIdAsync(Guid id);
}