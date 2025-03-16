using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace C4FAMS.Models
{
    public class DangKySuKien
    {
        [Key]
        public int MaDangKy { get; set; }

        [Required]
        public int MaSuKien { get; set; }

        [ForeignKey("MaSuKien")]
        public virtual SuKien SuKien { get; set; }

        [Required]
        public string MSSV { get; set; }

        [ForeignKey("MSSV")]
        public virtual CuuSinhVien CuuSinhVien { get; set; }

        [Required]
        public DateTime NgayDangKy { get; set; } = DateTime.Now;

        public bool TrangThai { get; set; } = true; // true: đã đăng ký, false: hủy đăng ký
    }
}