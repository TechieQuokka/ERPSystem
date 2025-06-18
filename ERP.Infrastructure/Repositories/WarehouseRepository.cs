using ERP.Domain.Entities;
using ERP.Domain.Interfaces;
using ERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Repositories
{
    public class WarehouseRepository : Repository<Warehouse>, IWarehouseRepository
    {
        public WarehouseRepository(ERPDbContext context) : base(context)
        {
        }

        public async Task<Warehouse?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(w => w.Name == name);
        }

        public async Task<bool> IsNameUniqueAsync(string name, int? excludeId = null)
        {
            var query = _dbSet.Where(w => w.Name == name);
            if (excludeId.HasValue)
            {
                query = query.Where(w => w.Id != excludeId.Value);
            }
            return !await query.AnyAsync();
        }

        public override async Task<IEnumerable<Warehouse>> GetAllAsync()
        {
            return await _dbSet.OrderBy(w => w.Name).ToListAsync();
        }
    }
}
