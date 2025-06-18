using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERP.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        /// <summary>
        /// 제품별 재고 조회
        /// </summary>
        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<InventoryDto>>> GetInventoryByProduct(int productId)
        {
            var inventories = await _inventoryService.GetInventoryByProductAsync(productId);
            return Ok(inventories);
        }

        /// <summary>
        /// 창고별 재고 조회
        /// </summary>
        [HttpGet("warehouse/{warehouseId}")]
        public async Task<ActionResult<IEnumerable<InventoryDto>>> GetInventoryByWarehouse(int warehouseId)
        {
            var inventories = await _inventoryService.GetInventoryByWarehouseAsync(warehouseId);
            return Ok(inventories);
        }

        /// <summary>
        /// 재고 부족 제품 조회
        /// </summary>
        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<InventoryDto>>> GetLowStock()
        {
            var inventories = await _inventoryService.GetLowStockAsync();
            return Ok(inventories);
        }

        /// <summary>
        /// 재고 입고
        /// </summary>
        [HttpPost("receive/{productId}/{warehouseId}")]
        public async Task<ActionResult<InventoryDto>> Receive(int productId, int warehouseId, [FromBody] ReceiveInventoryDto receiveDto)
        {
            try
            {
                var inventory = await _inventoryService.ReceiveAsync(productId, warehouseId, receiveDto);
                return Ok(inventory);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 재고 출고
        /// </summary>
        [HttpPost("issue/{productId}/{warehouseId}")]
        public async Task<ActionResult<InventoryDto>> Issue(int productId, int warehouseId, [FromBody] IssueInventoryDto issueDto)
        {
            try
            {
                var inventory = await _inventoryService.IssueAsync(productId, warehouseId, issueDto);
                return Ok(inventory);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 창고 간 재고 이동
        /// </summary>
        [HttpPost("transfer/{productId}")]
        public async Task<ActionResult<InventoryTransactionDto>> Transfer(int productId, [FromBody] TransferInventoryDto transferDto)
        {
            try
            {
                var transaction = await _inventoryService.TransferAsync(productId, transferDto);
                return Ok(transaction);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 재고 조정
        /// </summary>
        [HttpPost("adjust/{productId}/{warehouseId}")]
        public async Task<ActionResult<InventoryDto>> Adjust(int productId, int warehouseId, [FromBody] AdjustInventoryDto adjustDto)
        {
            try
            {
                var inventory = await _inventoryService.AdjustAsync(productId, warehouseId, adjustDto);
                return Ok(inventory);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 재고 트랜잭션 내역 조회
        /// </summary>
        [HttpGet("transactions/{productId}")]
        public async Task<ActionResult<IEnumerable<InventoryTransactionDto>>> GetTransactionHistory(int productId, [FromQuery] int? warehouseId)
        {
            var transactions = await _inventoryService.GetTransactionHistoryAsync(productId, warehouseId);
            return Ok(transactions);
        }
    }
}
