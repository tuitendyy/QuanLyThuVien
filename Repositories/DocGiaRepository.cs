using System;
using System.Collections.Generic;
using System.Linq;
using QuanLyThuVien.Data;
using QuanLyThuVien.Helpers;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.Repositories
{
    public class DocGiaRepository
    {
        public List<DocGia> Search(string keyword)
        {
            keyword = keyword ?? "";
            using (var db = new LibraryContext())
            {
                return db.DocGias
                         .Where(d => d.MaDocGia.Contains(keyword) || d.HoTen.Contains(keyword) || d.Lop.Contains(keyword))
                         .OrderBy(d => d.MaDocGia).ToList();
            }
        }

        public DocGia Get(string maDocGia)
        {
            using (var db = new LibraryContext()) return db.DocGias.Find(maDocGia);
        }

        private static void Validate(DocGia d)
        {
            if (string.IsNullOrWhiteSpace(d.MaDocGia)) throw new BusinessException("Vui lòng nhập mã độc giả.");
            if (string.IsNullOrWhiteSpace(d.HoTen)) throw new BusinessException("Vui lòng nhập họ tên.");
            if (d.NgayHetHan.Date < DateTime.Today && d.TrangThai) throw new BusinessException("Ngày hết hạn thẻ phải từ hôm nay trở đi.");
        }

        public void Add(DocGia d)
        {
            Validate(d);
            using (var db = new LibraryContext())
            {
                if (db.DocGias.Any(x => x.MaDocGia == d.MaDocGia)) throw new BusinessException("Mã độc giả đã tồn tại. Nhấn 'Làm mới' rồi nhập mã khác.");
                d.NgayCapThe = DateTime.Today;
                db.DocGias.Add(d);
                db.SaveChanges();
            }
        }

        public void Update(DocGia d)
        {
            Validate(d);
            using (var db = new LibraryContext())
            {
                var e = db.DocGias.Find(d.MaDocGia);
                if (e == null) throw new BusinessException("Không tìm thấy độc giả.");
                e.HoTen = d.HoTen; e.NgaySinh = d.NgaySinh; e.GioiTinh = d.GioiTinh; e.Lop = d.Lop;
                e.Email = d.Email; e.SoDienThoai = d.SoDienThoai; e.NgayHetHan = d.NgayHetHan; e.TrangThai = d.TrangThai;
                db.SaveChanges();
            }
        }

        public void Delete(string maDocGia)
        {
            using (var db = new LibraryContext())
            {
                if (db.PhieuMuons.Any(p => p.MaDocGia == maDocGia))
                    throw new BusinessException("Độc giả đã có phiếu mượn nên không thể xóa. Hãy khóa thẻ (bỏ chọn 'Thẻ còn hiệu lực').");
                var e = db.DocGias.Find(maDocGia);
                if (e == null) return;
                db.DocGias.Remove(e);
                db.SaveChanges();
            }
        }
    }
}
