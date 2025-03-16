using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace C4FAMS.Models
{
    public class CongViec
    {
        [Key]
        public int MaCongViec { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "MSSV phải đúng 10 chữ số")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "MSSV phải là 10 chữ số")]
        public string MSSV { get; set; } = null!;

        [StringLength(100)]
        public string? ViTri { get; set; }

        [StringLength(100)]
        public string? CongTy { get; set; }

        [StringLength(100)]
        public string? LinhVuc { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? ThuNhap { get; set; }

        [ForeignKey("MSSV")]
        public CuuSinhVien CuuSinhVien { get; set; } = null!;
    }
}