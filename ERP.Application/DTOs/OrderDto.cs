using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "고객 ID는 필수입니다.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "주문 상태는 필수입니다.")]
        [StringLength(50, ErrorMessage = "주문 상태는 50자를 초과할 수 없습니다.")]
        public string Status { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "총액은 0 이상이어야 합니다.")]
        public decimal TotalAmount { get; set; }

        public List<OrderDetailDto> OrderDetails { get; set; } = new List<OrderDetailDto>();
    }
}
