using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class UpdateOrderStatusDto
    {
        [Required(ErrorMessage = "주문 상태는 필수입니다.")]
        [StringLength(50, ErrorMessage = "주문 상태는 50자를 초과할 수 없습니다.")]
        public string Status { get; set; } = string.Empty;
    }
}
