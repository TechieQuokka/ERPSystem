using ERP.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP.Application.Interfaces
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryDto>> GetInventoryByProductAsync(int productId);
        Task<IEnumerable<InventoryDto>> GetInventoryByWarehouseAsync(int warehouseId);
        Task<IEnumerable<InventoryDto>> GetLowStockAsync();
        Task<InventoryDto> ReceiveAsync(int productId, int warehouseId, ReceiveInventoryDto receiveDto);
        Task<InventoryDto> IssueAsync(int productId, int warehouseId, IssueInventoryDto issueDto);
        Task<InventoryTransactionDto> TransferAsync(int productId, TransferInventoryDto transferDto);
        Task<InventoryDto> AdjustAsync(int productId, int warehouseId, AdjustInventoryDto adjustDto);
        Task<IEnumerable<InventoryTransactionDto>> GetTransactionHistoryAsync(int productId, int? warehouseId);
    }
}
