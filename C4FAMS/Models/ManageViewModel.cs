using System.ComponentModel.DataAnnotations;

namespace C4FAMS.Models
{
    public class ManageViewModel
    {
        public string Email { get; set; } = null!;
        public string VaiTro { get; set; } = null!;
        public bool TrangThai { get; set; }
        public string? TenKhoa { get; set; }

        // Dùng cho thay đổi mật khẩu
        [DataType(DataType.Password)]
        public string? OldPassword { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Mật khẩu mới phải từ {2} đến {1} ký tự.", MinimumLength = 6)]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu mới và xác nhận mật khẩu không khớp.")]
        public string? ConfirmNewPassword { get; set; }
    }
}