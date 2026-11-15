using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuVien.Models
{
    [Table("TheLoai")]
    public class TheLoai
    {
        [Key] public int MaTheLoai { get; set; }
        [Required, StringLength(100)] public string TenTheLoai { get; set; }
    }
}
