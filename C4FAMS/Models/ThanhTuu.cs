using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace C4FAMS.Models
{
    public class ThanhTuu
    { 
        [Key]
        public int MaThanhTuu { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "MSSV phải đúng 10 chữ số")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "MSSV phải là 10 chữ số")]
        public string MSSV { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string TenThanhTuu { get; set; } = null!;

        [StringLength(500)]
        public string? MoTa { get; set; }

        [Required]
        [Range(1900, 9999, ErrorMessage = "Năm đạt được phải từ 1900 trở đi")]
        [CustomValidation(typeof(ThanhTuu), nameof(ValidateNamDatDuoc))]
        public int NamDatDuoc { get; set; }

        [ForeignKey("MSSV")]
        public CuuSinhVien CuuSinhVien { get; set; } = null!;

        public static ValidationResult? ValidateNamDatDuoc(int namDatDuoc, ValidationContext context)
        {
            var thanhTuu = (ThanhTuu)context.ObjectInstance;
            if (thanhTuu.CuuSinhVien != null && namDatDuoc < thanhTuu.CuuSinhVien.NamTotNghiep)
            {
                return new ValidationResult("Năm đạt thành tựu phải sau năm tốt nghiệp.");
            }
            return ValidationResult.Success;
        }
    }
}