using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace C4FAMS.Models
{
    public class NguoiDung : IdentityUser<int>
    {
        [Required(ErrorMessage = "Vai trò là bắt buộc")]
        [StringLength(20)]
        public string VaiTro { get; set; } = null!;

        public int? MaKhoa { get; set; }

        [Required]
        public bool TrangThai { get; set; } = true;
    }
}