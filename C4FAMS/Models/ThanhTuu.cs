using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace C4FAMS.Models
{
    public class ThanhTuu
    {
        [Key]
        public int MaThanhTuu { get; set; }

        [Required]
        [StringLength(20)]
        public string MSSV { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string TenThanhTuu { get; set; } = null!;

        [StringLength(500)]
        public string? MoTa { get; set; }

        [Required]
        [Range(1900, 2025)]
        public int NamDatDuoc { get; set; }

        [ForeignKey("MSSV")]
        public CuuSinhVien CuuSinhVien { get; set; } = null!;
    }
}