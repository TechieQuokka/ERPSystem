using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class InventoryTransactionDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }

        [Required(ErrorMessage = "트랜잭션 유형은 필수입니다.")]
        public string TransactionType { get; set; } = string.Empty;

        public int Quantity { get; set; }
        public DateTime TransactionDate { get; set; }

        [StringLength(500, ErrorMessage = "사유는 500자를 초과할 수 없습니다.")]
        public string? Reason { get; set; }
    }
}
