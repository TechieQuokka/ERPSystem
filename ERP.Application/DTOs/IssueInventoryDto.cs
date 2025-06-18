using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class IssueInventoryDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "출고 수량은 1 이상이어야 합니다.")]
        public int Quantity { get; set; }

        [StringLength(500, ErrorMessage = "출고 사유는 500자를 초과할 수 없습니다.")]
        public string? Reason { get; set; }
    }
}
