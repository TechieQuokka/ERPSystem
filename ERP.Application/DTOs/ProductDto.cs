using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "제품명은 필수입니다.")]
        [StringLength(100, ErrorMessage = "제품명은 100자를 초과할 수 없습니다.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "SKU는 필수입니다.")]
        [StringLength(50, ErrorMessage = "SKU는 50자를 초과할 수 없습니다.")]
        public string SKU { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "설명은 1000자를 초과할 수 없습니다.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "가격은 필수입니다.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "가격은 0보다 커야 합니다.")]
        public decimal UnitPrice { get; set; }

        public DateTime CreatedAt { get; set; }

        // 재고 정보
        public InventoryDto? Inventory { get; set; }
    }

    public class CreateProductDto
    {
        [Required(ErrorMessage = "제품명은 필수입니다.")]
        [StringLength(100, ErrorMessage = "제품명은 100자를 초과할 수 없습니다.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "SKU는 필수입니다.")]
        [StringLength(50, ErrorMessage = "SKU는 50자를 초과할 수 없습니다.")]
        public string SKU { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "설명은 1000자를 초과할 수 없습니다.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "가격은 필수입니다.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "가격은 0보다 커야 합니다.")]
        public decimal UnitPrice { get; set; }
    }

    public class UpdateProductDto
    {
        [Required(ErrorMessage = "제품명은 필수입니다.")]
        [StringLength(100, ErrorMessage = "제품명은 100자를 초과할 수 없습니다.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "SKU는 필수입니다.")]
        [StringLength(50, ErrorMessage = "SKU는 50자를 초과할 수 없습니다.")]
        public string SKU { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "설명은 1000자를 초과할 수 없습니다.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "가격은 필수입니다.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "가격은 0보다 커야 합니다.")]
        public decimal UnitPrice { get; set; }
    }
}