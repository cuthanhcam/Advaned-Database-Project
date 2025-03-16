using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace C4FAMS.Models
{
    public class SuKienHinhAnh
    {
        [Key]
        public int MaHinhAnh { get; set; }

        [Required(ErrorMessage = "Mã sự kiện là bắt buộc")]
        public int MaSuKien { get; set; }

        [Required(ErrorMessage = "Đường dẫn hình ảnh là bắt buộc")]
        [StringLength(255, ErrorMessage = "Đường dẫn không được vượt quá 255 ký tự")]
        [RegularExpression(@"^[\w\-\./]+$", ErrorMessage = "Đường dẫn hình ảnh phải hợp lệ")]
        public string HinhAnh { get; set; } = null!;

        [ForeignKey("MaSuKien")]
        public virtual SuKien SuKien { get; set; } = null!;
    }
}