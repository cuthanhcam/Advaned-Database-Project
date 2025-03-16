using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace C4FAMS.Models
{
    public class ThanhTuuViewModel
    {
        public int MaThanhTuu { get; set; }
        [Required]
        [StringLength(100)]
        public string TenThanhTuu { get; set; } = null!;

        [StringLength(500)]
        public string? MoTa { get; set; }

        [Required]
        [Range(1900, 9999, ErrorMessage = "Năm đạt được phải từ 1900 trở đi")]
        public int NamDatDuoc { get; set; }
    }
}