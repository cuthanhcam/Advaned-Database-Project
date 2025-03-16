using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace C4FAMS.Models
{
    public class ThongBao
    {
        [Key]
        public int MaThongBao { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "MSSV phải đúng 10 chữ số")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "MSSV phải là 10 chữ số")]
        public string MSSV { get; set; } = null!;

        [Required]
        [StringLength(500)]
        public string NoiDung { get; set; } = null!;

        [Required]
        public DateTime NgayGui { get; set; }

        [Required]
        public bool TrangThai { get; set; } = false;

        [ForeignKey("MSSV")]
        public CuuSinhVien CuuSinhVien { get; set; } = null!;
    }
}