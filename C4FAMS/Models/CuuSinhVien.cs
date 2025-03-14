using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace C4FAMS.Models
{
    public class CuuSinhVien
    {
        [Key]
        [StringLength(20)]
        [ForeignKey("SinhVien")]
        public string MSSV { get; set; } = null!;

        [Required(ErrorMessage = "Năm tốt nghiệp là bắt buộc")]
        [Range(1900, 2025, ErrorMessage = "Năm tốt nghiệp phải từ 1900 đến 2025")]
        public int NamTotNghiep { get; set; }

        [Required(ErrorMessage = "Xếp loại tốt nghiệp là bắt buộc")]
        [StringLength(50)]
        public string XepLoaiTotNghiep { get; set; } = null!;

        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public string? DiaChiHienTai { get; set; }

        [Required(ErrorMessage = "Hình thức liên lạc là bắt buộc")]
        [StringLength(20)]
        public string HinhThucLienLac { get; set; } = null!;

        public SinhVien SinhVien { get; set; } = null!;
    }
}