using ERP.Domain.Entities;

namespace ERP.Domain.Interfaces
{
    public interface IInventoryTransactionRepository : IRepository<InventoryTransaction>
    {
        Task<IEnumerable<InventoryTransaction>> GetByProductAndWarehouseAsync(int productId, int? warehouseId);
    }
}
