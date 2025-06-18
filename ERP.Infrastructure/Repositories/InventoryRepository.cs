using ERP.Domain.Entities;
using ERP.Domain.Interfaces;
using ERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Repositories
{
    public class InventoryRepository : Repository<Inventory>, IInventoryRepository
    {
        public InventoryRepository(ERPDbContext context) : base(context)
        {
        }

        public async Task<Inventory?> GetByProductAndWarehouseAsync(int productId, int warehouseId)
        {
            return await _dbSet
                .Include(i => i.Product)
                .Include(i => i.Warehouse)
                .FirstOrDefaultAsync(i => i.ProductId == productId && i.WarehouseId == warehouseId);
        }

        public async Task<IEnumerable<Inventory>> GetAllByProductAsync(int productId)
        {
            return await _dbSet
                .Include(i => i.Warehouse)
                .Where(i => i.ProductId == productId)
                .OrderBy(i => i.Warehouse.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Inventory>> GetAllByWarehouseAsync(int warehouseId)
        {
            return await _dbSet
                .Include(i => i.Product)
                .Where(i => i.WarehouseId == warehouseId)
                .OrderBy(i => i.Product.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Inventory>> GetLowStockAsync()
        {
            return await _dbSet
                .Include(i => i.Product)
                .Include(i => i.Warehouse)
                .Where(i => i.Quantity <= i.MinimumStock)
                .OrderBy(i => i.Product.Name)
                .ToListAsync();
        }
    }
}
