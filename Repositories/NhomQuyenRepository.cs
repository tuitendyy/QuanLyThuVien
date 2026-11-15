using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using QuanLyThuVien.Data;
using QuanLyThuVien.Helpers;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.Repositories
{
    public class NhomQuyenRepository
    {
        public List<NhomQuyen> GetNhoms()
        {
            using (var db = new LibraryContext()) return db.NhomQuyens.OrderBy(n => n.MaNhom).ToList();
        }

        public List<QuyenHan> GetQuyens()
        {
            using (var db = new LibraryContext()) return db.QuyenHans.OrderBy(q => q.MaQuyen).ToList();
        }

        public List<string> GetMaQuyenCuaNhom(int maNhom)
        {
            using (var db = new LibraryContext())
            {
                var n = db.NhomQuyens.Include(x => x.QuyenHans).FirstOrDefault(x => x.MaNhom == maNhom);
                return n == null ? new List<string>() : n.QuyenHans.Select(q => q.MaQuyen).ToList();
            }
        }

        public void AddNhom(string ten, string moTa)
        {
            if (string.IsNullOrWhiteSpace(ten)) throw new BusinessException("Vui lòng nhập tên nhóm.");
            using (var db = new LibraryContext())
            {
                if (db.NhomQuyens.Any(n => n.TenNhom == ten.Trim())) throw new BusinessException("Tên nhóm đã tồn tại.");
                db.NhomQuyens.Add(new NhomQuyen { TenNhom = ten.Trim(), MoTa = moTa });
                db.SaveChanges();
            }
        }

        public void DeleteNhom(int maNhom)
        {
            using (var db = new LibraryContext())
            {
                var n = db.NhomQuyens.Include(x => x.QuyenHans).FirstOrDefault(x => x.MaNhom == maNhom);
                if (n == null) return;
                if (n.TenNhom == "Admin") throw new BusinessException("Không thể xóa nhóm Admin.");
                if (db.NguoiDungs.Any(u => u.MaNhom == maNhom)) throw new BusinessException("Nhóm đang có người dùng, không thể xóa.");
                n.QuyenHans.Clear();
                db.NhomQuyens.Remove(n);
                db.SaveChanges();
            }
        }

        /// <summary>Ghi lại tập quyền của nhóm (xóa cũ, thêm mới) trong cùng một lần SaveChanges.</summary>
        public void SaveQuyen(int maNhom, List<string> maQuyens)
        {
            using (var db = new LibraryContext())
            {
                var n = db.NhomQuyens.Include(x => x.QuyenHans).FirstOrDefault(x => x.MaNhom == maNhom);
                if (n == null) throw new BusinessException("Không tìm thấy nhóm.");
                if (n.TenNhom == "Admin" && (!maQuyens.Contains("QL_USER") || !maQuyens.Contains("QL_NHOMQUYEN")))
                    throw new BusinessException("Nhóm Admin phải luôn giữ quyền Quản lý người dùng và Quản lý nhóm quyền.");
                n.QuyenHans.Clear();
                foreach (var q in db.QuyenHans.Where(x => maQuyens.Contains(x.MaQuyen)).ToList())
                    n.QuyenHans.Add(q);
                db.SaveChanges();
            }
        }
    }
}
