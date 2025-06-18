using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class InventoryDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "수량은 0 이상이어야 합니다.")]
        public int Quantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "최소 재고는 0 이상이어야 합니다.")]
        public int MinimumStock { get; set; }

        public DateTime LastUpdated { get; set; }

        // 재고 부족 여부
        public bool IsLowStock => Quantity <= MinimumStock;

        // 재고 상태
        public string StockStatus => IsLowStock ? "부족" : "충분";
    }

    public class UpdateInventoryDto
    {
        [Range(0, int.MaxValue, ErrorMessage = "수량은 0 이상이어야 합니다.")]
        public int Quantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "최소 재고는 0 이상이어야 합니다.")]
        public int MinimumStock { get; set; }
    }
}