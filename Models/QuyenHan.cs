using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuVien.Models
{
    [Table("QuyenHan")]
    public class QuyenHan
    {
        [Key, StringLength(30)] public string MaQuyen { get; set; }
        [Required, StringLength(100)] public string TenQuyen { get; set; }

        public virtual ICollection<NhomQuyen> NhomQuyens { get; set; } = new HashSet<NhomQuyen>();
    }
}
