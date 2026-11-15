using System;
using System.Collections.Generic;
using System.Linq;
using QuanLyThuVien.Data;
using QuanLyThuVien.Helpers;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.Repositories
{
    /// <summary>
    /// NGHIỆP VỤ MƯỢN - TRẢ (có Transaction).
    /// Mọi thay đổi trong một lần lập phiếu / trả sách nằm trong cùng một giao dịch:
    /// lỗi ở bất kỳ bước nào -> Rollback, dữ liệu không bị sai lệch.
    /// </summary>
    public class MuonTraService
    {
        /// <summary>Lập phiếu mượn. Trả về mã phiếu vừa tạo.</summary>
        public int LapPhieuMuon(string maDocGia, int maND, List<int> maSachs, string ghiChu)
        {
            if (string.IsNullOrWhiteSpace(maDocGia)) throw new BusinessException("Vui lòng nhập mã độc giả.");
            if (maSachs == null || maSachs.Count == 0) throw new BusinessException("Chưa chọn sách nào để mượn.");
            maSachs = maSachs.Distinct().ToList();

            using (var db = new LibraryContext())
            using (var tx = db.Database.BeginTransaction())
            {
                try
                {
                    DateTime today = DateTime.Today;

                    // 1. Kiểm tra thẻ độc giả
                    var dg = db.DocGias.Find(maDocGia);
                    if (dg == null) throw new BusinessException("Không tìm thấy độc giả có mã " + maDocGia + ".");
                    if (!dg.TrangThai) throw new BusinessException("Thẻ độc giả đang bị khóa.");
                    if (dg.NgayHetHan.Date < today) throw new BusinessException("Thẻ độc giả đã hết hạn từ " + dg.NgayHetHan.ToString("dd/MM/yyyy") + ".");

                    // 2. Không cho mượn nếu còn nợ phạt cũ hoặc đang giữ sách quá hạn
                    bool conNoPhat = db.ChiTietPhieuMuons.Any(c => c.PhieuMuon.MaDocGia == maDocGia
                                         && c.NgayTra != null && c.TienPhat > 0 && !c.DaThuPhat);
                    if (conNoPhat) throw new BusinessException("Độc giả còn nợ tiền phạt. Vui lòng thu phạt trước khi cho mượn tiếp.");
                    bool quaHan = db.ChiTietPhieuMuons.Any(c => c.PhieuMuon.MaDocGia == maDocGia
                                         && c.NgayTra == null && c.NgayHenTra < today);
                    if (quaHan) throw new BusinessException("Độc giả đang giữ sách quá hạn chưa trả.");

                    // 3. Giới hạn số sách mượn cùng lúc
                    int toiDa = QuyDinhRepository.GetValue(db, QuyDinhRepository.SoSachToiDa);
                    int soNgay = QuyDinhRepository.GetValue(db, QuyDinhRepository.SoNgayMuon);
                    var dangMuonIds = db.ChiTietPhieuMuons
                                        .Where(c => c.PhieuMuon.MaDocGia == maDocGia && c.NgayTra == null)
                                        .Select(c => c.MaSach).ToList();
                    if (dangMuonIds.Count + maSachs.Count > toiDa)
                        throw new BusinessException("Mỗi độc giả chỉ được mượn tối đa " + toiDa + " cuốn. Hiện đang mượn " + dangMuonIds.Count + " cuốn.");

                    // 4. Tạo phiếu + chi tiết, trừ tồn kho từng cuốn
                    var pm = new PhieuMuon { MaDocGia = maDocGia, MaND = maND, NgayMuon = DateTime.Now, GhiChu = ghiChu };
                    foreach (int maSach in maSachs)
                    {
                        var s = db.Sachs.Find(maSach);
                        if (s == null) throw new BusinessException("Sách có mã " + maSach + " không tồn tại.");
                        if (dangMuonIds.Contains(maSach)) throw new BusinessException("Độc giả đang mượn cuốn '" + s.TenSach + "' rồi.");
                        if (s.SoLuongCon <= 0) throw new BusinessException("Sách '" + s.TenSach + "' đã hết, không thể cho mượn.");
                        s.SoLuongCon -= 1;
                        pm.ChiTiets.Add(new ChiTietPhieuMuon { MaSach = maSach, NgayHenTra = today.AddDays(soNgay) });
                    }
                    db.PhieuMuons.Add(pm);
                    db.SaveChanges();
                    tx.Commit();
                    return pm.MaPhieu;
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        /// <summary>Trả một cuốn sách. Trả về tiền phạt (0 nếu đúng hạn).</summary>
        public decimal TraSach(int maPhieu, int maSach)
        {
            using (var db = new LibraryContext())
            using (var tx = db.Database.BeginTransaction())
            {
                try
                {
                    var ct = db.ChiTietPhieuMuons.Find(maPhieu, maSach);
                    if (ct == null) throw new BusinessException("Không tìm thấy dòng mượn sách này.");
                    if (ct.NgayTra != null) throw new BusinessException("Cuốn sách này đã được trả rồi.");

                    int phi = QuyDinhRepository.GetValue(db, QuyDinhRepository.PhiPhatMoiNgay);
                    DateTime now = DateTime.Now;
                    int soNgayTre = Math.Max(0, (now.Date - ct.NgayHenTra.Date).Days);

                    ct.NgayTra = now;
                    ct.TienPhat = soNgayTre * phi;
                    ct.DaThuPhat = ct.TienPhat == 0;

                    var s = db.Sachs.Find(maSach);
                    s.SoLuongCon += 1;                      // cộng lại tồn kho

                    db.SaveChanges();
                    tx.Commit();
                    return ct.TienPhat;
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        /// <summary>Đánh dấu đã thu tiền phạt.</summary>
        public void ThuPhat(int maPhieu, int maSach)
        {
            using (var db = new LibraryContext())
            {
                var ct = db.ChiTietPhieuMuons.Find(maPhieu, maSach);
                if (ct == null) throw new BusinessException("Không tìm thấy dòng mượn sách này.");
                if (ct.NgayTra == null) throw new BusinessException("Sách chưa trả, hãy trả sách trước.");
                ct.DaThuPhat = true;
                db.SaveChanges();
            }
        }
    }
}
