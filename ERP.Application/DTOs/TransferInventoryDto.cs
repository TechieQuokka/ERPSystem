using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class TransferInventoryDto
    {
        [Required(ErrorMessage = "대상 창고 ID는 필수입니다.")]
        public int ToWarehouseId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "이동 수량은 1 이상이어야 합니다.")]
        public int Quantity { get; set; }

        [StringLength(500, ErrorMessage = "이동 사유는 500자를 초과할 수 없습니다.")]
        public string? Reason { get; set; }
    }
}
