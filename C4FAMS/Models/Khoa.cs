using System.ComponentModel.DataAnnotations;

namespace C4FAMS.Models
{
    public class Khoa
    {
        [Key]
        public int MaKhoa { get; set; }

        [Required(ErrorMessage = "Tên khoa là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên khoa không được vượt quá 100 ký tự")]
        public string TenKhoa { get; set; } = null!;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = null!;

        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Số điện thoại phải là số")]
        public string? SoDienThoai { get; set; }
    }
}