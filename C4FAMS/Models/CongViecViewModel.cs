using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace C4FAMS.Models
{
    public class CongViecViewModel
    {
        public int MaCongViec { get; set; }
        [StringLength(100)]
        public string? ViTri { get; set; }

        [StringLength(100)]
        public string? CongTy { get; set; }

        [StringLength(100)]
        public string? LinhVuc { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? ThuNhap { get; set; } // Có thể để trống
    }
}