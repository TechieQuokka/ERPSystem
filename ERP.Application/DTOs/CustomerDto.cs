using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class CustomerDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "고객명은 필수입니다.")]
        [StringLength(100, ErrorMessage = "고객명은 100자를 초과할 수 없습니다.")]
        public string Name { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "올바른 이메일 형식을 입력해주세요.")]
        [StringLength(100, ErrorMessage = "이메일은 100자를 초과할 수 없습니다.")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "올바른 전화번호 형식을 입력해주세요.")]
        [StringLength(20, ErrorMessage = "전화번호는 20자를 초과할 수 없습니다.")]
        public string? Phone { get; set; }

        [StringLength(500, ErrorMessage = "주소는 500자를 초과할 수 없습니다.")]
        public string? Address { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class CreateCustomerDto
    {
        [Required(ErrorMessage = "고객명은 필수입니다.")]
        [StringLength(100, ErrorMessage = "고객명은 100자를 초과할 수 없습니다.")]
        public string Name { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "올바른 이메일 형식을 입력해주세요.")]
        [StringLength(100, ErrorMessage = "이메일은 100자를 초과할 수 없습니다.")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "올바른 전화번호 형식을 입력해주세요.")]
        [StringLength(20, ErrorMessage = "전화번호는 20자를 초과할 수 없습니다.")]
        public string? Phone { get; set; }

        [StringLength(500, ErrorMessage = "주소는 500자를 초과할 수 없습니다.")]
        public string? Address { get; set; }
    }

    public class UpdateCustomerDto
    {
        [Required(ErrorMessage = "고객명은 필수입니다.")]
        [StringLength(100, ErrorMessage = "고객명은 100자를 초과할 수 없습니다.")]
        public string Name { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "올바른 이메일 형식을 입력해주세요.")]
        [StringLength(100, ErrorMessage = "이메일은 100자를 초과할 수 없습니다.")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "올바른 전화번호 형식을 입력해주세요.")]
        [StringLength(20, ErrorMessage = "전화번호는 20자를 초과할 수 없습니다.")]
        public string? Phone { get; set; }

        [StringLength(500, ErrorMessage = "주소는 500자를 초과할 수 없습니다.")]
        public string? Address { get; set; }
    }
}