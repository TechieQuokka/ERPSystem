using ERP.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP.Application.Interfaces
{
    public interface IWarehouseService
    {
        Task<IEnumerable<WarehouseDto>> GetAllWarehousesAsync();
        Task<WarehouseDto?> GetWarehouseByIdAsync(int id);
        Task<WarehouseDto> CreateWarehouseAsync(CreateWarehouseDto createWarehouseDto);
        Task<bool> UpdateWarehouseAsync(int id, UpdateWarehouseDto updateWarehouseDto);
        Task<bool> DeleteWarehouseAsync(int id);
    }
}
