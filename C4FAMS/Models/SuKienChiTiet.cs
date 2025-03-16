using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace C4FAMS.Models
{
    public class SuKienChiTiet
    {
        [Key]
        [ForeignKey("SuKien")]
        public int MaSuKien { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? MoTa { get; set; }

        public string? NoiDung { get; set; } // NVARCHAR(MAX) không giới hạn độ dài

        [StringLength(255, ErrorMessage = "Thumbnail không được vượt quá 255 ký tự")]
        [RegularExpression(@"^[\w\-\./]+$", ErrorMessage = "Thumbnail phải là đường dẫn hợp lệ")]
        public string? Thumbnail { get; set; }

        public virtual SuKien SuKien { get; set; } = null!;
    }
}