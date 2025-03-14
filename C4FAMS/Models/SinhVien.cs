using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace C4FAMS.Models
{
    public class SinhVien
    {
        [Key]
        [StringLength(20, ErrorMessage = "MSSV không được vượt quá 20 ký tự")]
        public string MSSV { get; set; } = null!;

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        public string HoTen { get; set; } = null!;

        [Required(ErrorMessage = "Giới tính là bắt buộc")]
        public bool GioiTinh { get; set; }

        [Required(ErrorMessage = "Ngày sinh là bắt buộc")]
        [DataType(DataType.Date)]
        public DateTime NgaySinh { get; set; }

        [StringLength(200, ErrorMessage = "Quê quán không được vượt quá 200 ký tự")]
        public string? QueQuan { get; set; }

        [Required(ErrorMessage = "Ngành học là bắt buộc")]
        [StringLength(100, ErrorMessage = "Ngành học không được vượt quá 100 ký tự")]
        public string NganhHoc { get; set; } = null!;

        [Required(ErrorMessage = "Mã khoa là bắt buộc")]
        public int MaKhoa { get; set; }

        [ForeignKey("MaKhoa")]
        public Khoa Khoa { get; set; } = null!;

        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string? Email { get; set; }

        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Số điện thoại phải là số")]
        public string? SoDienThoai { get; set; }
    }
}