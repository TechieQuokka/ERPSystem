using ERP.Domain.Entities;

namespace ERP.Domain.Interfaces
{
    public interface IInventoryRepository : IRepository<Inventory>
    {
        Task<Inventory?> GetByProductAndWarehouseAsync(int productId, int warehouseId);
        Task<IEnumerable<Inventory>> GetAllByProductAsync(int productId);
        Task<IEnumerable<Inventory>> GetAllByWarehouseAsync(int warehouseId);
        Task<IEnumerable<Inventory>> GetLowStockAsync();
    }
}
