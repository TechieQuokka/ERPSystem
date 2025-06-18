using ERP.Domain.Entities;

namespace ERP.Domain.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer?> GetByEmailAsync(string email);
        Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null);
        Task<bool> ExistsAsync(int id);
    }
}