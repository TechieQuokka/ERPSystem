using ERP.Domain.Entities;

namespace ERP.Domain.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product?> GetBySkuAsync(string sku);
        Task<bool> IsSkuUniqueAsync(string sku, int? excludeId = null);
        Task<IEnumerable<Product>> GetProductsWithInventoryAsync();
        Task<Product?> GetByIdWithInventoryAsync(int id);
    }
}