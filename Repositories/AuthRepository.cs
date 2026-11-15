using System.Data.Entity;
using System.Linq;
using QuanLyThuVien.Data;
using QuanLyThuVien.Helpers;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.Repositories
{
    public class AuthRepository
    {
        /// <summary>Đăng nhập: so sánh hash SHA-256; nạp luôn nhóm quyền + danh sách quyền.</summary>
        public NguoiDung Login(string tenDangNhap, string matKhau)
        {
            string hash = SecurityHelper.Hash(matKhau);
            using (var db = new LibraryContext())
            {
                return db.NguoiDungs
                         .Include(u => u.NhomQuyen.QuyenHans)
                         .FirstOrDefault(u => u.TenDangNhap == tenDangNhap && u.MatKhau == hash && u.HoatDong);
            }
        }

        public void DoiMatKhau(int maND, string matKhauCu, string matKhauMoi)
        {
            using (var db = new LibraryContext())
            {
                var u = db.NguoiDungs.Find(maND);
                if (u == null) throw new BusinessException("Không tìm thấy tài khoản.");
                if (u.MatKhau != SecurityHelper.Hash(matKhauCu)) throw new BusinessException("Mật khẩu cũ không đúng.");
                u.MatKhau = SecurityHelper.Hash(matKhauMoi);
                db.SaveChanges();
            }
        }
    }
}
