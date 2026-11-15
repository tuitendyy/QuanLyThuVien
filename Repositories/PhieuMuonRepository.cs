using System;
using System.Collections.Generic;
using System.Linq;
using QuanLyThuVien.Data;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.Repositories
{
    /// <summary>Các truy vấn đọc dữ liệu mượn - trả (LINQ Join / Where / Sum).</summary>
    public class PhieuMuonRepository
    {
        // Truy vấn nền: ChiTietPhieuMuon JOIN PhieuMuon JOIN Sach JOIN DocGia
        private static IQueryable<ChiTietMuonDto> Query(LibraryContext db)
        {
            return from ct in db.ChiTietPhieuMuons
                   join pm in db.PhieuMuons on ct.MaPhieu equals pm.MaPhieu
                   join s in db.Sachs on ct.MaSach equals s.MaSach
                   join dg in db.DocGias on pm.MaDocGia equals dg.MaDocGia
                   select new ChiTietMuonDto
                   {
                       MaPhieu = ct.MaPhieu, MaSach = ct.MaSach, TenSach = s.TenSach,
                       MaDocGia = dg.MaDocGia, HoTen = dg.HoTen, NgayMuon = pm.NgayMuon,
                       NgayHenTra = ct.NgayHenTra, NgayTra = ct.NgayTra,
                       TienPhat = ct.TienPhat, DaThuPhat = ct.DaThuPhat
                   };
        }

        // Tính trạng thái và tiền phạt dự kiến (sách quá hạn chưa trả)
        private static List<ChiTietMuonDto> Tinh(List<ChiTietMuonDto> list, int phiMoiNgay)
        {
            DateTime today = DateTime.Today;
            foreach (var x in list)
            {
                if (x.NgayTra == null)
                {
                    if (x.NgayHenTra.Date < today)
                    {
                        x.TrangThai = "Quá hạn";
                        x.TienPhat = (today - x.NgayHenTra.Date).Days * phiMoiNgay;   // dự kiến
                    }
                    else x.TrangThai = "Đang mượn";
                }
                else if (x.TienPhat > 0 && !x.DaThuPhat) x.TrangThai = "Còn nợ phạt";
                else x.TrangThai = "Đã trả";
            }
            return list;
        }

        public List<ChiTietMuonDto> GetDangMuonVaNo(string maDocGia)
        {
            using (var db = new LibraryContext())
            {
                int phi = QuyDinhRepository.GetValue(db, QuyDinhRepository.PhiPhatMoiNgay);
                var list = Query(db)
                    .Where(x => x.MaDocGia == maDocGia && (x.NgayTra == null || (x.TienPhat > 0 && !x.DaThuPhat)))
                    .OrderBy(x => x.NgayHenTra).ToList();
                return Tinh(list, phi);
            }
        }

        public List<ChiTietMuonDto> GetLichSu(string maDocGia)
        {
            using (var db = new LibraryContext())
            {
                int phi = QuyDinhRepository.GetValue(db, QuyDinhRepository.PhiPhatMoiNgay);
                var list = Query(db).Where(x => x.MaDocGia == maDocGia)
                                    .OrderByDescending(x => x.NgayMuon).ToList();
                return Tinh(list, phi);
            }
        }

        public List<ChiTietMuonDto> GetChiTiet(int maPhieu)
        {
            using (var db = new LibraryContext())
            {
                int phi = QuyDinhRepository.GetValue(db, QuyDinhRepository.PhiPhatMoiNgay);
                return Tinh(Query(db).Where(x => x.MaPhieu == maPhieu).ToList(), phi);
            }
        }

        public List<ChiTietMuonDto> GetChiTietTheoNgay(DateTime tuNgay, DateTime denNgay)
        {
            DateTime ngayBatDau = tuNgay.Date, ngayKetThuc = denNgay.Date.AddDays(1);
            using (var db = new LibraryContext())
            {
                int phi = QuyDinhRepository.GetValue(db, QuyDinhRepository.PhiPhatMoiNgay);
                var list = Query(db).Where(x => x.NgayMuon >= ngayBatDau && x.NgayMuon < ngayKetThuc)
                                    .OrderBy(x => x.NgayMuon).ThenBy(x => x.MaPhieu).ToList();
                return Tinh(list, phi);
            }
        }

        public List<PhieuMuonDto> GetPhieu(DateTime tuNgay, DateTime denNgay)
        {
            DateTime ngayBatDau = tuNgay.Date, ngayKetThuc = denNgay.Date.AddDays(1);
            using (var db = new LibraryContext())
            {
                var q = from pm in db.PhieuMuons
                        join dg in db.DocGias on pm.MaDocGia equals dg.MaDocGia
                        join nd in db.NguoiDungs on pm.MaND equals nd.MaND
                        where pm.NgayMuon >= ngayBatDau && pm.NgayMuon < ngayKetThuc
                        orderby pm.NgayMuon descending
                        select new PhieuMuonDto
                        {
                            MaPhieu = pm.MaPhieu, MaDocGia = dg.MaDocGia, HoTen = dg.HoTen,
                            ThuThu = nd.HoTen, NgayMuon = pm.NgayMuon, SoSach = pm.ChiTiets.Count()
                        };
                return q.ToList();
            }
        }

        public PhieuMuonDto GetPhieu(int maPhieu)
        {
            using (var db = new LibraryContext())
            {
                var q = from pm in db.PhieuMuons
                        join dg in db.DocGias on pm.MaDocGia equals dg.MaDocGia
                        join nd in db.NguoiDungs on pm.MaND equals nd.MaND
                        where pm.MaPhieu == maPhieu
                        select new PhieuMuonDto
                        {
                            MaPhieu = pm.MaPhieu, MaDocGia = dg.MaDocGia, HoTen = dg.HoTen,
                            ThuThu = nd.HoTen, NgayMuon = pm.NgayMuon, SoSach = pm.ChiTiets.Count()
                        };
                return q.FirstOrDefault();
            }
        }

        /// <summary>Số sách đang mượn, tổng tiền phạt còn nợ (dùng Sum) và số sách đang quá hạn của một độc giả.</summary>
        public void ThongKe(string maDocGia, out int dangMuon, out decimal tienNo, out int quaHan)
        {
            DateTime today = DateTime.Today;
            using (var db = new LibraryContext())
            {
                dangMuon = db.ChiTietPhieuMuons.Count(c => c.PhieuMuon.MaDocGia == maDocGia && c.NgayTra == null);
                quaHan = db.ChiTietPhieuMuons.Count(c => c.PhieuMuon.MaDocGia == maDocGia && c.NgayTra == null && c.NgayHenTra < today);
                tienNo = db.ChiTietPhieuMuons
                           .Where(c => c.PhieuMuon.MaDocGia == maDocGia && c.NgayTra != null && !c.DaThuPhat)
                           .Select(c => (decimal?)c.TienPhat).Sum() ?? 0;
            }
        }
    }
}
