using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuVien.Models
{
    [Table("ChiTietPhieuMuon")]
    public class ChiTietPhieuMuon
    {
        [Key, Column(Order = 0)] public int MaPhieu { get; set; }
        [Key, Column(Order = 1)] public int MaSach { get; set; }
        [Column(TypeName = "date")] public DateTime NgayHenTra { get; set; }
        public DateTime? NgayTra { get; set; }          // null = chưa trả
        public decimal TienPhat { get; set; }
        public bool DaThuPhat { get; set; }

        public virtual PhieuMuon PhieuMuon { get; set; }
        public virtual Sach Sach { get; set; }
    }
}
