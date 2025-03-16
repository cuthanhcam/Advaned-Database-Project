using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace C4FAMS.Models
{
    public class DangKySuKien
    {
        [Key]
        public int MaDangKy { get; set; }

        [Required(ErrorMessage = "MSSV là bắt buộc")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "MSSV phải đúng 10 chữ số")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "MSSV phải là 10 chữ số")]
        public string MSSV { get; set; } = null!;

        [Required(ErrorMessage = "Mã sự kiện là bắt buộc")]
        public int MaSuKien { get; set; }

        [Required(ErrorMessage = "Ngày đăng ký là bắt buộc")]
        [DataType(DataType.DateTime)]
        public DateTime NgayDangKy { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        [StringLength(50)]
        public string TrangThai { get; set; } = "Đã đăng ký"; // Mặc định là "Đã đăng ký"

        [ForeignKey("MSSV")]
        public virtual CuuSinhVien CuuSinhVien { get; set; } = null!;

        [ForeignKey("MaSuKien")]
        public virtual SuKien SuKien { get; set; } = null!;
    }
}