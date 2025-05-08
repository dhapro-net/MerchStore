using System.Threading.Tasks;
using MerchStore.Domain.Entities;

namespace MerchStore.Application.Services.Interfaces
{
    public interface IProductCatalogService
    {
        Task<Product> GetProductByIdAsync(string productId);
    }
}