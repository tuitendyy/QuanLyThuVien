using System.Collections.Generic;
using System.Linq;
using QuanLyThuVien.Data;
using QuanLyThuVien.Helpers;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.Repositories
{
    public class SachRepository
    {
        /// <summary>
        /// Tìm sách bằng LINQ Join 4 bảng (Sach - TheLoai - TacGia - NhaXuatBan).
        /// </summary>
        public List<SachDto> Search(string keyword, int? maTheLoai = null, bool chiConSach = false)
        {
            keyword = keyword ?? "";
            using (var db = new LibraryContext())
            {
                var q = from s in db.Sachs
                        join tl in db.TheLoais on s.MaTheLoai equals tl.MaTheLoai
                        join tg in db.TacGias on s.MaTacGia equals tg.MaTacGia
                        join nxb in db.NhaXuatBans on s.MaNXB equals nxb.MaNXB
                        where (s.TenSach.Contains(keyword) || tg.TenTacGia.Contains(keyword) || tl.TenTheLoai.Contains(keyword))
                              && (!maTheLoai.HasValue || s.MaTheLoai == maTheLoai.Value)
                              && (!chiConSach || s.SoLuongCon > 0)
                        orderby s.TenSach
                        select new SachDto
                        {
                            MaSach = s.MaSach, TenSach = s.TenSach,
                            MaTheLoai = s.MaTheLoai, TheLoai = tl.TenTheLoai,
                            MaTacGia = s.MaTacGia, TacGia = tg.TenTacGia,
                            MaNXB = s.MaNXB, NhaXuatBan = nxb.TenNXB,
                            NamXuatBan = s.NamXuatBan, GiaBia = s.GiaBia,
                            SoLuongTong = s.SoLuongTong, SoLuongCon = s.SoLuongCon, ViTri = s.ViTri
                        };
                return q.ToList();
            }
        }

        public void Save(Sach s)
        {
            if (string.IsNullOrWhiteSpace(s.TenSach)) throw new BusinessException("Vui lòng nhập tên sách.");
            if (s.SoLuongTong <= 0) throw new BusinessException("Tổng số lượng phải lớn hơn 0.");

            using (var db = new LibraryContext())
            {
                if (s.MaSach == 0)
                {
                    s.SoLuongCon = s.SoLuongTong;      // sách mới: tất cả đều đang trên kệ
                    db.Sachs.Add(s);
                }
                else
                {
                    var e = db.Sachs.Find(s.MaSach);
                    if (e == null) throw new BusinessException("Không tìm thấy sách.");
                    int dangMuon = e.SoLuongTong - e.SoLuongCon;
                    if (s.SoLuongTong < dangMuon)
                        throw new BusinessException("Đang có " + dangMuon + " cuốn được mượn, tổng số lượng không thể nhỏ hơn.");
                    e.TenSach = s.TenSach; e.MaTheLoai = s.MaTheLoai; e.MaTacGia = s.MaTacGia; e.MaNXB = s.MaNXB;
                    e.NamXuatBan = s.NamXuatBan; e.GiaBia = s.GiaBia; e.ViTri = s.ViTri;
                    e.SoLuongTong = s.SoLuongTong;
                    e.SoLuongCon = s.SoLuongTong - dangMuon;
                }
                db.SaveChanges();
            }
        }

        public void Delete(int maSach)
        {
            using (var db = new LibraryContext())
            {
                if (db.ChiTietPhieuMuons.Any(c => c.MaSach == maSach))
                    throw new BusinessException("Sách đã từng được mượn nên không thể xóa.");
                var e = db.Sachs.Find(maSach);
                if (e == null) return;
                db.Sachs.Remove(e);
                db.SaveChanges();
            }
        }
    }
}
