using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuVien.Models
{
    [Table("TacGia")]
    public class TacGia
    {
        [Key] public int MaTacGia { get; set; }
        [Required, StringLength(100)] public string TenTacGia { get; set; }
        [StringLength(50)] public string QuocTich { get; set; }
    }
}
