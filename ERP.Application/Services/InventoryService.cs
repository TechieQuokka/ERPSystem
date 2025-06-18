using AutoMapper;
using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Domain.Entities;
using ERP.Domain.Interfaces;

namespace ERP.Application.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IInventoryTransactionRepository _transactionRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IMapper _mapper;

        public InventoryService(
            IInventoryRepository inventoryRepository,
            IInventoryTransactionRepository transactionRepository,
            IWarehouseRepository warehouseRepository,
            IMapper mapper)
        {
            _inventoryRepository = inventoryRepository;
            _transactionRepository = transactionRepository;
            _warehouseRepository = warehouseRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<InventoryDto>> GetInventoryByProductAsync(int productId)
        {
            var inventories = await _inventoryRepository.GetAllByProductAsync(productId);
            return _mapper.Map<IEnumerable<InventoryDto>>(inventories);
        }

        public async Task<IEnumerable<InventoryDto>> GetInventoryByWarehouseAsync(int warehouseId)
        {
            var inventories = await _inventoryRepository.GetAllByWarehouseAsync(warehouseId);
            return _mapper.Map<IEnumerable<InventoryDto>>(inventories);
        }

        public async Task<IEnumerable<InventoryDto>> GetLowStockAsync()
        {
            var inventories = await _inventoryRepository.GetLowStockAsync();
            return _mapper.Map<IEnumerable<InventoryDto>>(inventories);
        }

        public async Task<InventoryDto> ReceiveAsync(int productId, int warehouseId, ReceiveInventoryDto receiveDto)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(warehouseId);
            if (warehouse == null)
                throw new InvalidOperationException($"창고 ID {warehouseId}를 찾을 수 없습니다.");

            using var transaction = await _inventoryRepository.BeginTransactionAsync();
            try
            {
                var inventory = await _inventoryRepository.GetByProductAndWarehouseAsync(productId, warehouseId);
                if (inventory == null)
                {
                    inventory = new Inventory
                    {
                        ProductId = productId,
                        WarehouseId = warehouseId,
                        Quantity = 0,
                        MinimumStock = 0
                    };
                    await _inventoryRepository.AddAsync(inventory);
                }

                inventory.Quantity += receiveDto.Quantity;
                inventory.LastUpdated = DateTime.UtcNow;

                var inventoryTransaction = new InventoryTransaction
                {
                    ProductId = productId,
                    WarehouseId = warehouseId,
                    TransactionType = "Receive",
                    Quantity = receiveDto.Quantity,
                    Reason = receiveDto.Reason
                };
                await _transactionRepository.AddAsync(inventoryTransaction);

                await _inventoryRepository.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<InventoryDto>(inventory);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<InventoryDto> IssueAsync(int productId, int warehouseId, IssueInventoryDto issueDto)
        {
            var inventory = await _inventoryRepository.GetByProductAndWarehouseAsync(productId, warehouseId);
            if (inventory == null || inventory.Quantity < issueDto.Quantity)
            {
                throw new InvalidOperationException($"재고가 부족합니다: 제품 ID {productId}, 창고 ID {warehouseId}");
            }

            // 트랜잭션 없이 비즈니스 로직만 처리
            inventory.Quantity -= issueDto.Quantity;
            inventory.LastUpdated = DateTime.UtcNow;

            var inventoryTransaction = new InventoryTransaction
            {
                ProductId = productId,
                WarehouseId = warehouseId,
                TransactionType = "Issue",
                Quantity = issueDto.Quantity,
                Reason = issueDto.Reason
            };

            await _transactionRepository.AddAsync(inventoryTransaction);
            // SaveChanges 호출하지 않음 - 상위에서 처리

            return _mapper.Map<InventoryDto>(inventory);
        }

        public async Task<InventoryDto> IssueWithTransactionAsync(int productId, int warehouseId, IssueInventoryDto issueDto)
        {
            using var transaction = await _inventoryRepository.BeginTransactionAsync();
            try
            {
                var result = await IssueAsync(productId, warehouseId, issueDto);
                await _inventoryRepository.SaveChangesAsync();
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<InventoryTransactionDto> TransferAsync(int productId, TransferInventoryDto transferDto)
        {
            var fromInventory = await _inventoryRepository.GetByProductAndWarehouseAsync(productId, transferDto.ToWarehouseId);
            if (fromInventory == null || fromInventory.Quantity < transferDto.Quantity)
            {
                throw new InvalidOperationException($"출발 창고의 재고가 부족합니다: 제품 ID {productId}, 창고 ID {transferDto.ToWarehouseId}");
            }

            var toWarehouse = await _warehouseRepository.GetByIdAsync(transferDto.ToWarehouseId);
            if (toWarehouse == null)
                throw new InvalidOperationException($"대상 창고 ID {transferDto.ToWarehouseId}를 찾을 수 없습니다.");

            using var transaction = await _inventoryRepository.BeginTransactionAsync();
            try
            {
                var toInventory = await _inventoryRepository.GetByProductAndWarehouseAsync(productId, transferDto.ToWarehouseId);
                if (toInventory == null)
                {
                    toInventory = new Inventory
                    {
                        ProductId = productId,
                        WarehouseId = transferDto.ToWarehouseId,
                        Quantity = 0,
                        MinimumStock = 0
                    };
                    await _inventoryRepository.AddAsync(toInventory);
                }

                fromInventory.Quantity -= transferDto.Quantity;
                fromInventory.LastUpdated = DateTime.UtcNow;
                toInventory.Quantity += transferDto.Quantity;
                toInventory.LastUpdated = DateTime.UtcNow;

                var inventoryTransaction = new InventoryTransaction
                {
                    ProductId = productId,
                    WarehouseId = transferDto.ToWarehouseId,
                    TransactionType = "Transfer",
                    Quantity = transferDto.Quantity,
                    Reason = transferDto.Reason
                };
                await _transactionRepository.AddAsync(inventoryTransaction);

                await _inventoryRepository.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<InventoryTransactionDto>(inventoryTransaction);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<InventoryDto> AdjustAsync(int productId, int warehouseId, AdjustInventoryDto adjustDto)
        {
            var inventory = await _inventoryRepository.GetByProductAndWarehouseAsync(productId, warehouseId);
            if (inventory == null)
            {
                throw new InvalidOperationException($"재고를 찾을 수 없습니다: 제품 ID {productId}, 창고 ID {warehouseId}");
            }

            using var transaction = await _inventoryRepository.BeginTransactionAsync();
            try
            {
                var oldQuantity = inventory.Quantity;
                inventory.Quantity = adjustDto.NewQuantity;
                inventory.LastUpdated = DateTime.UtcNow;

                var inventoryTransaction = new InventoryTransaction
                {
                    ProductId = productId,
                    WarehouseId = warehouseId,
                    TransactionType = "Adjust",
                    Quantity = adjustDto.NewQuantity - oldQuantity,
                    Reason = adjustDto.Reason
                };
                await _transactionRepository.AddAsync(inventoryTransaction);

                await _inventoryRepository.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<InventoryDto>(inventory);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<InventoryTransactionDto>> GetTransactionHistoryAsync(int productId, int? warehouseId)
        {
            var transactions = await _transactionRepository.GetByProductAndWarehouseAsync(productId, warehouseId);
            return _mapper.Map<IEnumerable<InventoryTransactionDto>>(transactions);
        }
    }
}
