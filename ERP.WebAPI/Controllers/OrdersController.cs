using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERP.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// 모든 주문 조회
        /// </summary>
        /// <returns>주문 목록</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        /// <summary>
        /// 고객별 주문 조회
        /// </summary>
        /// <param name="customerId">고객 ID</param>
        /// <returns>고객의 주문 목록</returns>
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByCustomer(int customerId)
        {
            var orders = await _orderService.GetOrdersByCustomerAsync(customerId);
            return Ok(orders);
        }

        /// <summary>
        /// 주문 상세 조회
        /// </summary>
        /// <param name="id">주문 ID</param>
        /// <returns>주문 정보</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound(new { message = $"주문 ID {id}를 찾을 수 없습니다." });
            return Ok(order);
        }

        /// <summary>
        /// 주문 생성
        /// </summary>
        /// <param name="createOrderDto">주문 생성 데이터</param>
        /// <returns>생성된 주문 정보</returns>
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            try
            {
                var order = await _orderService.CreateOrderAsync(createOrderDto);
                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 주문 상태 업데이트
        /// </summary>
        /// <param name="id">주문 ID</param>
        /// <param name="statusDto">상태 업데이트 데이터</param>
        /// <returns>업데이트 성공 여부</returns>
        [HttpPut("{id}/status")]
        public async Task<ActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto statusDto)
        {
            try
            {
                var updated = await _orderService.UpdateOrderStatusAsync(id, statusDto);
                if (!updated)
                    return NotFound(new { message = $"주문 ID {id}를 찾을 수 없습니다." });
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 주문 취소
        /// </summary>
        /// <param name="id">주문 ID</param>
        /// <returns>취소 성공 여부</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> CancelOrder(int id)
        {
            try
            {
                var cancelled = await _orderService.CancelOrderAsync(id);
                if (!cancelled)
                    return NotFound(new { message = $"주문 ID {id}를 찾을 수 없습니다." });
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}