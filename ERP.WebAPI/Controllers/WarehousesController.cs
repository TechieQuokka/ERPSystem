using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERP.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public WarehousesController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        /// <summary>
        /// 모든 창고 조회
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WarehouseDto>>> GetWarehouses()
        {
            var warehouses = await _warehouseService.GetAllWarehousesAsync();
            return Ok(warehouses);
        }

        /// <summary>
        /// 창고 상세 조회
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<WarehouseDto>> GetWarehouse(int id)
        {
            var warehouse = await _warehouseService.GetWarehouseByIdAsync(id);
            if (warehouse == null)
            {
                return NotFound($"ID {id}에 해당하는 창고를 찾을 수 없습니다.");
            }
            return Ok(warehouse);
        }

        /// <summary>
        /// 새 창고 생성
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<WarehouseDto>> CreateWarehouse(CreateWarehouseDto createWarehouseDto)
        {
            try
            {
                var warehouse = await _warehouseService.CreateWarehouseAsync(createWarehouseDto);
                return CreatedAtAction(nameof(GetWarehouse), new { id = warehouse.Id }, warehouse);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 창고 정보 수정
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWarehouse(int id, UpdateWarehouseDto updateWarehouseDto)
        {
            try
            {
                var result = await _warehouseService.UpdateWarehouseAsync(id, updateWarehouseDto);
                if (!result)
                {
                    return NotFound($"ID {id}에 해당하는 창고를 찾을 수 없습니다.");
                }
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 창고 삭제
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWarehouse(int id)
        {
            try
            {
                var result = await _warehouseService.DeleteWarehouseAsync(id);
                if (!result)
                {
                    return NotFound($"ID {id}에 해당하는 창고를 찾을 수 없습니다.");
                }
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
