using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuVien.Models
{
    [Table("NguoiDung")]
    public class NguoiDung
    {
        [Key] public int MaND { get; set; }
        [Required, StringLength(50)] public string TenDangNhap { get; set; }
        [Required, StringLength(64)] public string MatKhau { get; set; }   // SHA-256 HEX
        [Required, StringLength(100)] public string HoTen { get; set; }
        [StringLength(100)] public string Email { get; set; }
        public int MaNhom { get; set; }
        public bool HoatDong { get; set; }
        public DateTime NgayTao { get; set; }
        [StringLength(20)] public string MaDocGia { get; set; }            // tài khoản độc giả gắn với thẻ độc giả

        public virtual NhomQuyen NhomQuyen { get; set; }
    }
}
