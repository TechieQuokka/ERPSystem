using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class OrderDetailDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "제품 ID는 필수입니다.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "창고 ID는 필수입니다.")]
        public int WarehouseId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "수량은 1 이상이어야 합니다.")]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "단가는 0 이상이어야 합니다.")]
        public decimal UnitPrice { get; set; }
    }
}
