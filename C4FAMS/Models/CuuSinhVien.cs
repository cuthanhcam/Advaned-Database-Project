using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace C4FAMS.Models
{
    public class CuuSinhVien
    { 
        [Key]
        [ForeignKey("SinhVien")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "MSSV phải đúng 10 chữ số")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "MSSV phải là 10 chữ số")]
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
        [StringLength(100)]
        public string HinhThucLienLac { get; set; } = null!;

        public SinhVien SinhVien { get; set; } = null!;

        // Navigation cho quan hệ 1-nhiều
        public virtual ICollection<CongViec> CongViec { get; set; } = new List<CongViec>();
        public virtual ICollection<ThanhTuu> ThanhTuu { get; set; } = new List<ThanhTuu>();
    }
}