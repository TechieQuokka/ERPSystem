using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class AdjustInventoryDto
    {
        [Range(0, int.MaxValue, ErrorMessage = "조정 후 수량은 0 이상이어야 합니다.")]
        public int NewQuantity { get; set; }

        [Required(ErrorMessage = "조정 사유는 필수입니다.")]
        [StringLength(500, ErrorMessage = "조정 사유는 500자를 초과할 수 없습니다.")]
        public string Reason { get; set; } = string.Empty;
    }
}
