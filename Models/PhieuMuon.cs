using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuVien.Models
{
    [Table("PhieuMuon")]
    public class PhieuMuon
    {
        [Key] public int MaPhieu { get; set; }
        [Required, StringLength(20)] public string MaDocGia { get; set; }
        public int MaND { get; set; }
        public DateTime NgayMuon { get; set; }
        [StringLength(200)] public string GhiChu { get; set; }

        public virtual DocGia DocGia { get; set; }
        public virtual NguoiDung NguoiDung { get; set; }
        public virtual ICollection<ChiTietPhieuMuon> ChiTiets { get; set; } = new HashSet<ChiTietPhieuMuon>();
    }
}
