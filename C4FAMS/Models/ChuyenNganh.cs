using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace C4FAMS.Models
{
    public class ChuyenNganh
    {
        [Key]
        public int MaChuyenNganh { get; set; }

        [Required(ErrorMessage = "Tên chuyên ngành là bắt buộc")]
        [StringLength(255)]
        public string TenChuyenNganh { get; set; } = null!;

        [Required]
        public int MaKhoa { get; set; }

        [ForeignKey("MaKhoa")]
        public Khoa Khoa { get; set; } = null!;
    }
}