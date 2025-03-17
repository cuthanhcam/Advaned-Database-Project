using System.ComponentModel.DataAnnotations;

namespace C4FAMS.Models
{
    public class ManageCuuSinhVienViewModel
    {
        [Required(ErrorMessage = "Năm tốt nghiệp là bắt buộc")]
        [Range(1900, 2025, ErrorMessage = "Năm tốt nghiệp phải từ 1900 đến 2025")]
        public int NamTotNghiep { get; set; }

        [Required(ErrorMessage = "Xếp loại tốt nghiệp là bắt buộc")]
        [StringLength(50)]
        public string XepLoaiTotNghiep { get; set; } = null!;

        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public string? DiaChiHienTai { get; set; }

        [Required(ErrorMessage = "Hình thức liên lạc là bắt buộc")]
        [StringLength(100, ErrorMessage = "Hình thức liên lạc không được vượt quá 100 ký tự")]
        public string HinhThucLienLac { get; set; } = null!;

        public IEnumerable<CongViecViewModel> CongViecs { get; set; } = new List<CongViecViewModel>();
        public IEnumerable<ThanhTuuViewModel> ThanhTuus { get; set; } = new List<ThanhTuuViewModel>();
    }
}