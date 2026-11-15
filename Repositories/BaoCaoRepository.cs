using System;
using System.Collections.Generic;
using System.Linq;
using QuanLyThuVien.Data;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.Repositories
{
    public class BaoCaoRepository
    {
        /// <summary>Dữ liệu cho báo cáo mượn trả + tiền phạt trong khoảng ngày.</summary>
        public List<BaoCaoRow> GetMuonTra(DateTime tuNgay, DateTime denNgay)
        {
            var list = new PhieuMuonRepository().GetChiTietTheoNgay(tuNgay, denNgay);
            return list.Select((c, i) => BaoCaoRow.From(c, i + 1)).ToList();
        }

        /// <summary>Top sách được mượn nhiều nhất (GroupBy + Join) - dùng cho biểu đồ trang chủ.</summary>
        public List<TopSachDto> TopSachMuon(int top)
        {
            using (var db = new LibraryContext())
            {
                var q = from g in db.ChiTietPhieuMuons.GroupBy(ct => ct.MaSach)
                                    .Select(x => new { MaSach = x.Key, SoLan = x.Count() })
                        join s in db.Sachs on g.MaSach equals s.MaSach
                        orderby g.SoLan descending
                        select new TopSachDto { TenSach = s.TenSach, SoLan = g.SoLan };
                return q.Take(top).ToList();
            }
        }
    }
}
