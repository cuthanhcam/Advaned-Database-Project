using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace C4FAMS.Models
{
    public class SuKien
    {
        [Key]
        public int MaSuKien { get; set; }

        [Required(ErrorMessage = "Tên sự kiện là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên sự kiện không được vượt quá 100 ký tự")]
        public string TenSuKien { get; set; } = null!;

        [Required(ErrorMessage = "Ngày tổ chức là bắt buộc")]
        [DataType(DataType.DateTime)]
        public DateTime NgayToChuc { get; set; }

        [Required(ErrorMessage = "Địa điểm là bắt buộc")]
        [StringLength(200, ErrorMessage = "Địa điểm không được vượt quá 200 ký tự")]
        public string DiaDiem { get; set; } = null!;

        // [Required(ErrorMessage = "Mã khoa là bắt buộc")] // Tạm thời bỏ [Required] để test
        public int? MaKhoa { get; set; } // cho phép null

        [ForeignKey("MaKhoa")]
        public Khoa? Khoa { get; set; }

        public virtual ICollection<SuKienHinhAnh> SuKienHinhAnhs { get; set; } = new List<SuKienHinhAnh>(); // 1-n
    }
}