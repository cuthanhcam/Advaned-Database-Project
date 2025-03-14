using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace C4FAMS.Models
{
    public class DangKySuKien
    {
        [Key]
        public int MaDangKy { get; set; }

        [Required(ErrorMessage = "MSSV là bắt buộc")]
        [StringLength(20)]
        public string MSSV { get; set; } = null!;

        [Required(ErrorMessage = "Mã sự kiện là bắt buộc")]
        public int MaSuKien { get; set; }

        [Required(ErrorMessage = "Ngày đăng ký là bắt buộc")]
        [DataType(DataType.DateTime)]
        public DateTime NgayDangKy { get; set; }

        [ForeignKey("MSSV")]
        public CuuSinhVien CuuSinhVien { get; set; } = null!;

        [ForeignKey("MaSuKien")]
        public SuKien SuKien { get; set; } = null!;
    }
}