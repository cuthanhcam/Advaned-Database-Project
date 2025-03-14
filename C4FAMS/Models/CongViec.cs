using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace C4FAMS.Models
{
    public class CongViec
    {
        [Key]
        public int MaCongViec { get; set; }

        [Required]
        [StringLength(20)]
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