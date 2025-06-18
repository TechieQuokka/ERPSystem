using ERP.Domain.Entities;

namespace ERP.Domain.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetAllWithDetailsAsync();
        Task<IEnumerable<Order>> GetByCustomerAsync(int customerId);
        Task<Order?> GetByIdWithDetailsAsync(int id);
    }
}
