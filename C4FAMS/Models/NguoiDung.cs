using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace C4FAMS.Models
{
    public class NguoiDung : IdentityUser<int>
    {
        [Required(ErrorMessage = "Vai trò là bắt buộc")]
        [StringLength(20)]
        public string VaiTro { get; set; } = null!;

        [Required(ErrorMessage = "Mã khoa là bắt buộc", AllowEmptyStrings = false)]
        public int? MaKhoa { get; set; }

        [ForeignKey("MaKhoa")]
        public virtual Khoa? Khoa { get; set; }

        [Required]
        public bool TrangThai { get; set; } = true;

        public string? MSSV { get; set; }
        [ForeignKey("MSSV")]
        public SinhVien? SinhVien { get; set; }
    }
}