using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace C4FAMS.Models
{
    public class Khoa
    {
        [Key]
        public int MaKhoa { get; set; }

        [Required(ErrorMessage = "Tên khoa là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên khoa không được vượt quá 100 ký tự")]
        public string TenKhoa { get; set; } = null!;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = null!;

        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự")]
        [RegularExpression(@"^[0-9]{10,15}$", ErrorMessage = "Số điện thoại phải là 10-15 chữ số")]
        public string? SoDienThoai { get; set; }

        // Quan hệ 1-n với ChuyenNganh
        public virtual ICollection<ChuyenNganh> ChuyenNganh { get; set; } = new List<ChuyenNganh>();
        // Quan hệ 1-n với SuKien
        public virtual ICollection<SuKien> SuKiens { get; set; } = new List<SuKien>();
    }
}