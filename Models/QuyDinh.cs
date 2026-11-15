using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuVien.Models
{
    [Table("QuyDinh")]
    public class QuyDinh
    {
        [Key, StringLength(50)] public string TenQuyDinh { get; set; }
        public int GiaTri { get; set; }
        [StringLength(200)] public string MoTa { get; set; }
    }
}
