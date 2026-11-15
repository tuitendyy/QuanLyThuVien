using System.Collections.Generic;
using System.Linq;
using QuanLyThuVien.Data;
using QuanLyThuVien.Helpers;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.Repositories
{
    /// <summary>Đọc/ghi các quy định của thư viện (số sách tối đa, số ngày mượn, tiền phạt/ngày).</summary>
    public class QuyDinhRepository
    {
        public const string SoSachToiDa = "SoSachToiDa";
        public const string SoNgayMuon = "SoNgayMuon";
        public const string PhiPhatMoiNgay = "PhiPhatMoiNgay";

        public static int GetValue(LibraryContext db, string key)
        {
            var q = db.QuyDinhs.Find(key);
            if (q == null) throw new BusinessException("Thiếu cấu hình quy định: " + key);
            return q.GiaTri;
        }

        public static int GetValue(string key)
        {
            using (var db = new LibraryContext()) return GetValue(db, key);
        }

        public List<QuyDinh> GetAll()
        {
            using (var db = new LibraryContext()) return db.QuyDinhs.ToList();
        }

        public void Update(string key, int value)
        {
            if (value <= 0) throw new BusinessException("Giá trị quy định phải lớn hơn 0.");
            using (var db = new LibraryContext())
            {
                var q = db.QuyDinhs.Find(key);
                if (q == null) throw new BusinessException("Không tìm thấy quy định: " + key);
                q.GiaTri = value;
                db.SaveChanges();
            }
        }
    }
}
