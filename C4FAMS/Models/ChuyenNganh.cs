using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace C4FAMS.Models
{
    public class ChuyenNganh
    {
        [Key]
        public int MaChuyenNganh { get; set; }

        [Required(ErrorMessage = "Tên chuyên ngành là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên chuyên ngành không được vượt quá 100 ký tự")]
        public string TenChuyenNganh { get; set; } = null!;

        [Required(ErrorMessage = "Mã khoa là bắt buộc")]
        public int MaKhoa { get; set; }

        [ForeignKey("MaKhoa")]
        public Khoa? Khoa { get; set; }
    }
}