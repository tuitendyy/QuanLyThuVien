using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuVien.Models
{
    [Table("NhomQuyen")]
    public class NhomQuyen
    {
        [Key] public int MaNhom { get; set; }
        [Required, StringLength(50)] public string TenNhom { get; set; }
        [StringLength(200)] public string MoTa { get; set; }

        public virtual ICollection<NguoiDung> NguoiDungs { get; set; } = new HashSet<NguoiDung>();
        // Quan hệ nhiều-nhiều qua bảng ChiTietQuyen (cấu hình trong LibraryContext)
        public virtual ICollection<QuyenHan> QuyenHans { get; set; } = new HashSet<QuyenHan>();
    }
}
