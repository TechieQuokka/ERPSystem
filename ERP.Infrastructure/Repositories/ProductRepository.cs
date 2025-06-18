using Microsoft.EntityFrameworkCore;
using ERP.Domain.Entities;
using ERP.Domain.Interfaces;
using ERP.Infrastructure.Data;

namespace ERP.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ERPDbContext context) : base(context)
        {
        }

        public async Task<Product?> GetBySkuAsync(string sku)
        {
            return await _dbSet
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.SKU == sku);
        }

        public async Task<bool> IsSkuUniqueAsync(string sku, int? excludeId = null)
        {
            var query = _dbSet.Where(p => p.SKU == sku);
            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }
            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsWithInventoryAsync()
        {
            return await _dbSet
                .Include(p => p.Inventory)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdWithInventoryAsync(int id)
        {
            return await _dbSet
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public override async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await GetProductsWithInventoryAsync();
        }

        public override async Task<Product?> GetByIdAsync(int id)
        {
            return await GetByIdWithInventoryAsync(id);
        }
    }
}