using ERP.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP.Application.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<IEnumerable<OrderDto>> GetOrdersByCustomerAsync(int customerId);
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);
        Task<bool> UpdateOrderStatusAsync(int id, UpdateOrderStatusDto statusDto);
        Task<bool> CancelOrderAsync(int id);
    }
}
