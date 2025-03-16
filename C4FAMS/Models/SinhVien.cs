using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace C4FAMS.Models
{
    public enum TrangThaiSinhVien
    {
        ConHoc = 0,
        BaoLuu = 1,
        ThoiHoc = 2,
        DaTotNghiep = 3
    }

    public class SinhVien
    {
        [Key]
        [Required(ErrorMessage = "MSSV là bắt buộc")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "MSSV phải là 10 chữ số")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "MSSV phải đúng 10 chữ số")]
        public string MSSV { get; set; } = null!;

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        public string HoTen { get; set; } = null!;

        [Required(ErrorMessage = "Giới tính là bắt buộc")]
        public bool GioiTinh { get; set; }

        [Required(ErrorMessage = "Ngày sinh là bắt buộc")]
        [DataType(DataType.Date)]
        public DateTime NgaySinh { get; set; }

        [StringLength(100, ErrorMessage = "Quê quán không được vượt quá 100 ký tự")]
        public string? QueQuan { get; set; }

        [Required(ErrorMessage = "Mã chuyên ngành là bắt buộc")]
        public int MaChuyenNganh { get; set; }

        // [ForeignKey("MaChuyenNganh")]
        // public ChuyenNganh ChuyenNganh { get; set; } = null!;

        [ForeignKey("MaChuyenNganh")]
        public ChuyenNganh? ChuyenNganh { get; set; } // Navigation property cho phép null để tránh lỗi khi thêm sinh viên mới

        [Required(ErrorMessage = "Khóa học là bắt buộc")]
        [StringLength(10, ErrorMessage = "Khóa học không được vượt quá 10 ký tự")]
        [RegularExpression(@"^K\d{2}$", ErrorMessage = "Khóa học phải có định dạng KXX (ví dụ: K22)")]
        public string KhoaHoc { get; set; } = null!;

        [Required(ErrorMessage = "Lớp là bắt buộc")]
        [StringLength(20, ErrorMessage = "Lớp không được vượt quá 20 ký tự")]
        public string Lop { get; set; } = null!;

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        public TrangThaiSinhVien TrangThai { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Số điện thoại phải đúng 10 chữ số")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Số điện thoại phải là 10 chữ số")]
        public string SoDienThoai { get; set; } = null!;
    }
}