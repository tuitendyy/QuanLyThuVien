using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuVien.Models
{
    [Table("Sach")]
    public class Sach
    {
        [Key] public int MaSach { get; set; }
        [Required, StringLength(200)] public string TenSach { get; set; }
        public int MaTheLoai { get; set; }
        public int MaTacGia { get; set; }
        public int MaNXB { get; set; }
        public int? NamXuatBan { get; set; }
        public decimal GiaBia { get; set; }
        public int SoLuongTong { get; set; }
        public int SoLuongCon { get; set; }       // số cuốn còn trên kệ
        [StringLength(50)] public string ViTri { get; set; }

        public virtual TheLoai TheLoai { get; set; }
        public virtual TacGia TacGia { get; set; }
        public virtual NhaXuatBan NhaXuatBan { get; set; }
    }
}
