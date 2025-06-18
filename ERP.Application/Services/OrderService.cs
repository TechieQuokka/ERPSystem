using AutoMapper;
using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Domain.Entities;
using ERP.Domain.Interfaces;

namespace ERP.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IInventoryService _inventoryService;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            IInventoryService inventoryService,
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _inventoryService = inventoryService;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerAsync(int customerId)
        {
            var orders = await _orderRepository.GetByCustomerAsync(customerId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(id);
            return order == null ? null : _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            // 고객 유효성 검사
            var customer = await _customerRepository.GetByIdAsync(createOrderDto.CustomerId);
            if (customer == null)
                throw new InvalidOperationException($"고객 ID {createOrderDto.CustomerId}를 찾을 수 없습니다.");

            // 주문 항목 유효성 검사 및 재고 확인
            var orderDetails = new List<OrderDetail>();
            decimal totalAmount = 0;

            foreach (var detailDto in createOrderDto.OrderDetails)
            {
                var product = await _productRepository.GetByIdAsync(detailDto.ProductId);
                if (product == null)
                    throw new InvalidOperationException($"제품 ID {detailDto.ProductId}를 찾을 수 없습니다.");

                var inventory = await _inventoryService.GetInventoryByProductAsync(detailDto.ProductId);
                var warehouseInventory = inventory.FirstOrDefault(i => i.WarehouseId == detailDto.WarehouseId);
                if (warehouseInventory == null || warehouseInventory.Quantity < detailDto.Quantity)
                    throw new InvalidOperationException($"재고 부족: 제품 ID {detailDto.ProductId}, 창고 ID {detailDto.WarehouseId}");

                var orderDetail = new OrderDetail
                {
                    ProductId = detailDto.ProductId,
                    WarehouseId = detailDto.WarehouseId,
                    Quantity = detailDto.Quantity,
                    UnitPrice = product.UnitPrice
                };
                orderDetails.Add(orderDetail);
                totalAmount += detailDto.Quantity * product.UnitPrice;
            }

            // 트랜잭션 시작 - 모든 데이터베이스 작업을 하나의 트랜잭션으로 관리
            using var transaction = await _orderRepository.BeginTransactionAsync();
            try
            {
                // 주문 생성
                var order = new Order
                {
                    CustomerId = createOrderDto.CustomerId,
                    Status = "Pending",
                    TotalAmount = totalAmount,
                    OrderDetails = orderDetails
                };

                await _orderRepository.AddAsync(order);
                await _orderRepository.SaveChangesAsync(); // 주문 저장하여 order.Id 생성

                // 재고 감소 - 트랜잭션 없이 호출 (현재 트랜잭션 사용)
                foreach (var detail in orderDetails)
                {
                    await _inventoryService.IssueAsync(detail.ProductId, detail.WarehouseId, new IssueInventoryDto
                    {
                        Quantity = detail.Quantity,
                        Reason = $"주문 ID {order.Id}에 의한 출고"
                    });
                }

                // 모든 변경사항을 저장 (재고 변경사항 포함)
                await _orderRepository.SaveChangesAsync();

                // 트랜잭션 커밋
                await _orderRepository.CommitTransactionAsync(transaction);

                // 완성된 주문 정보 반환
                var createdOrder = await _orderRepository.GetByIdWithDetailsAsync(order.Id);
                return _mapper.Map<OrderDto>(createdOrder);
            }
            catch
            {
                await _orderRepository.RollbackTransactionAsync(transaction);
                throw;
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int id, UpdateOrderStatusDto statusDto)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                return false;

            // 상태 전이 유효성 검사 (예: 단순화된 규칙)
            var validStatuses = new List<string> { "Pending", "Shipped", "Delivered", "Cancelled" };
            if (!validStatuses.Contains(statusDto.Status))
                throw new InvalidOperationException($"유효하지 않은 주문 상태: {statusDto.Status}");

            if (order.Status == "Cancelled" || order.Status == "Delivered")
                throw new InvalidOperationException($"주문 상태를 변경할 수 없습니다: 현재 상태 {order.Status}");

            using var transaction = await _orderRepository.BeginTransactionAsync();
            try
            {
                order.Status = statusDto.Status;
                await _orderRepository.UpdateAsync(order);
                await _orderRepository.SaveChangesAsync();
                await _orderRepository.CommitTransactionAsync(transaction);
                return true;
            }
            catch
            {
                await _orderRepository.RollbackTransactionAsync(transaction);
                throw;
            }
        }

        public async Task<bool> CancelOrderAsync(int id)
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(id);
            if (order == null || order.Status == "Cancelled")
                return false;

            if (order.Status == "Delivered")
                throw new InvalidOperationException("배송 완료된 주문은 취소할 수 없습니다.");

            using var transaction = await _orderRepository.BeginTransactionAsync();
            try
            {
                // 주문 상태 변경
                order.Status = "Cancelled";
                await _orderRepository.UpdateAsync(order);

                // 재고 복구
                foreach (var detail in order.OrderDetails)
                {
                    await _inventoryService.ReceiveAsync(detail.ProductId, detail.WarehouseId, new ReceiveInventoryDto
                    {
                        Quantity = detail.Quantity,
                        Reason = $"주문 ID {id} 취소에 의한 재고 복구"
                    });
                }

                await _orderRepository.SaveChangesAsync();
                await _orderRepository.CommitTransactionAsync(transaction);
                return true;
            }
            catch
            {
                await _orderRepository.RollbackTransactionAsync(transaction);
                throw;
            }
        }
    }
}