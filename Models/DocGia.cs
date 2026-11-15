using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuVien.Models
{
    [Table("DocGia")]
    public class DocGia
    {
        [Key, StringLength(20)] public string MaDocGia { get; set; }
        [Required, StringLength(100)] public string HoTen { get; set; }
        [Column(TypeName = "date")] public DateTime? NgaySinh { get; set; }
        [StringLength(5)] public string GioiTinh { get; set; }
        [StringLength(50)] public string Lop { get; set; }
        [StringLength(100)] public string Email { get; set; }
        [StringLength(15)] public string SoDienThoai { get; set; }
        [Column(TypeName = "date")] public DateTime NgayCapThe { get; set; }
        [Column(TypeName = "date")] public DateTime NgayHetHan { get; set; }
        public bool TrangThai { get; set; }
    }
}
