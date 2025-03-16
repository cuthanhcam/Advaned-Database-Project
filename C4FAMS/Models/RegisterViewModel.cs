// Models/ViewModels/RegisterViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace C4FAMS.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "MSSV là bắt buộc")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "MSSV phải đúng 10 chữ số")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "MSSV phải là 10 chữ số")]
        public string MSSV { get; set; }

        [Required(ErrorMessage = "Năm tốt nghiệp là bắt buộc")]
        [Range(1900, 2025, ErrorMessage = "Năm tốt nghiệp phải từ 1900 đến 2025")]
        public int NamTotNghiep { get; set; }

        [Required(ErrorMessage = "Xếp loại tốt nghiệp là bắt buộc")]
        [StringLength(50)]
        public string XepLoaiTotNghiep { get; set; }

        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public string? DiaChiHienTai { get; set; }

        [Required(ErrorMessage = "Hình thức liên lạc là bắt buộc")]
        [StringLength(20)]
        public string HinhThucLienLac { get; set; }
    }
}