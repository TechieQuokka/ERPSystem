using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "고객 ID는 필수입니다.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "주문 항목은 최소 하나 이상이어야 합니다.")]
        [MinLength(1, ErrorMessage = "주문 항목은 최소 하나 이상이어야 합니다.")]
        public List<CreateOrderDetailDto> OrderDetails { get; set; } = new List<CreateOrderDetailDto>();
    }

    public class CreateOrderDetailDto
    {
        [Required(ErrorMessage = "제품 ID는 필수입니다.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "창고 ID는 필수입니다.")]
        public int WarehouseId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "수량은 1 이상이어야 합니다.")]
        public int Quantity { get; set; }
    }
}
