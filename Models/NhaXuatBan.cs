using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuVien.Models
{
    [Table("NhaXuatBan")]
    public class NhaXuatBan
    {
        [Key] public int MaNXB { get; set; }
        [Required, StringLength(150)] public string TenNXB { get; set; }
        [StringLength(200)] public string DiaChi { get; set; }
    }
}
