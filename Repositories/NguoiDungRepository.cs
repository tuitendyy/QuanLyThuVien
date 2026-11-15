using System.Collections.Generic;
using System.Linq;
using QuanLyThuVien.Data;
using QuanLyThuVien.Helpers;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.Repositories
{
    public class NguoiDungRepository
    {
        public List<NguoiDungDto> Search(string keyword)
        {
            keyword = keyword ?? "";
            using (var db = new LibraryContext())
            {
                var q = from u in db.NguoiDungs
                        join n in db.NhomQuyens on u.MaNhom equals n.MaNhom
                        where u.TenDangNhap.Contains(keyword) || u.HoTen.Contains(keyword)
                        orderby u.MaND
                        select new NguoiDungDto
                        {
                            MaND = u.MaND, TenDangNhap = u.TenDangNhap, HoTen = u.HoTen, Email = u.Email,
                            MaNhom = u.MaNhom, TenNhom = n.TenNhom, HoatDong = u.HoatDong, MaDocGia = u.MaDocGia
                        };
                return q.ToList();
            }
        }

        /// <summary>Thêm (MaND = 0) hoặc sửa người dùng. matKhauMoi để trống khi sửa = giữ mật khẩu cũ.</summary>
        public void Save(NguoiDung u, string matKhauMoi)
        {
            if (string.IsNullOrWhiteSpace(u.TenDangNhap)) throw new BusinessException("Vui lòng nhập tên đăng nhập.");
            if (string.IsNullOrWhiteSpace(u.HoTen)) throw new BusinessException("Vui lòng nhập họ tên.");

            using (var db = new LibraryContext())
            {
                if (db.NguoiDungs.Any(x => x.TenDangNhap == u.TenDangNhap && x.MaND != u.MaND))
                    throw new BusinessException("Tên đăng nhập đã tồn tại.");
                if (!string.IsNullOrEmpty(u.MaDocGia) && !db.DocGias.Any(d => d.MaDocGia == u.MaDocGia))
                    throw new BusinessException("Mã độc giả liên kết không tồn tại.");

                if (u.MaND == 0)
                {
                    if (string.IsNullOrEmpty(matKhauMoi) || matKhauMoi.Length < 6)
                        throw new BusinessException("Mật khẩu phải có ít nhất 6 ký tự.");
                    u.MatKhau = SecurityHelper.Hash(matKhauMoi);
                    u.NgayTao = System.DateTime.Now;
                    db.NguoiDungs.Add(u);
                }
                else
                {
                    var e = db.NguoiDungs.Find(u.MaND);
                    if (e == null) throw new BusinessException("Không tìm thấy người dùng.");
                    e.TenDangNhap = u.TenDangNhap;
                    e.HoTen = u.HoTen;
                    e.Email = u.Email;
                    e.MaNhom = u.MaNhom;
                    e.HoatDong = u.HoatDong;
                    e.MaDocGia = u.MaDocGia;
                    if (!string.IsNullOrEmpty(matKhauMoi))
                    {
                        if (matKhauMoi.Length < 6) throw new BusinessException("Mật khẩu phải có ít nhất 6 ký tự.");
                        e.MatKhau = SecurityHelper.Hash(matKhauMoi);
                    }
                }
                db.SaveChanges();
            }
        }

        public void Delete(int maND)
        {
            using (var db = new LibraryContext())
            {
                if (db.PhieuMuons.Any(p => p.MaND == maND))
                    throw new BusinessException("Người dùng này đã lập phiếu mượn nên không thể xóa. Hãy bỏ chọn 'Hoạt động' để khóa tài khoản.");
                var u = db.NguoiDungs.Find(maND);
                if (u == null) return;
                db.NguoiDungs.Remove(u);
                db.SaveChanges();
            }
        }
    }
}
