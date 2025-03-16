using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace C4FAMS.Models
{
    public class SuKienSinhVien
    {
        [Key]
        [Column(Order = 0)]
        public int MaSuKien { get; set; }

        [Key]
        [Column(Order = 1)]
        public string MSSV { get; set; } = null!;

        [ForeignKey("MaSuKien")]
        public SuKien SuKien { get; set; } = null!;

        [ForeignKey("MSSV")]
        public SinhVien SinhVien { get; set; } = null!;
    }
}