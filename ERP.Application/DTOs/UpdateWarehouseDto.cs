using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class UpdateWarehouseDto
    {
        [Required(ErrorMessage = "창고 이름은 필수입니다.")]
        [StringLength(100, ErrorMessage = "창고 이름은 100자를 초과할 수 없습니다.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "위치는 500자를 초과할 수 없습니다.")]
        public string? Location { get; set; }
    }
}
