using ERP.Domain.Entities;
using ERP.Domain.Interfaces;
using ERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Repositories
{
    public class InventoryTransactionRepository : Repository<InventoryTransaction>, IInventoryTransactionRepository
    {
        public InventoryTransactionRepository(ERPDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<InventoryTransaction>> GetByProductAndWarehouseAsync(int productId, int? warehouseId)
        {
            var query = _dbSet
                .Include(t => t.Product)
                .Include(t => t.Warehouse)
                .Where(t => t.ProductId == productId);

            if (warehouseId.HasValue)
            {
                query = query.Where(t => t.WarehouseId == warehouseId.Value);
            }

            return await query.OrderByDescending(t => t.TransactionDate).ToListAsync();
        }
    }
}
