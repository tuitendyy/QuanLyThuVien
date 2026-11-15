using System.Linq;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using QuanLyThuVien.Helpers;
using QuanLyThuVien.Models;
using QuanLyThuVien.Repositories;

namespace QuanLyThuVien.Forms
{
    /// <summary>In phiếu mượn bằng RDLC (ReportViewer).</summary>
    public class frmInPhieuMuon : Form
    {
        private readonly ReportViewer rv = new ReportViewer { Dock = DockStyle.Fill };
        private readonly int maPhieu;

        public frmInPhieuMuon(int maPhieu)
        {
            this.maPhieu = maPhieu;
            Text = "Phiếu mượn sách #" + maPhieu;
            Font = UI.Fnt;
            Size = new System.Drawing.Size(900, 700);
            StartPosition = FormStartPosition.CenterParent;
            Controls.Add(rv);
            Load += (s, e) => UI.Run(Nap);
        }

        private void Nap()
        {
            var repo = new PhieuMuonRepository();
            PhieuMuonDto p = repo.GetPhieu(maPhieu);
            if (p == null) throw new BusinessException("Không tìm thấy phiếu mượn.");
            var rows = repo.GetChiTiet(maPhieu).Select((c, i) => BaoCaoRow.From(c, i + 1)).ToList();

            rv.LocalReport.ReportEmbeddedResource = "QuanLyThuVien.Reports.PhieuMuon.rdlc";
            rv.LocalReport.DataSources.Clear();
            rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", rows));
            rv.LocalReport.SetParameters(new[]
            {
                new ReportParameter("MaPhieu", p.MaPhieu.ToString()),
                new ReportParameter("NgayMuon", p.NgayMuon.ToString("dd/MM/yyyy HH:mm")),
                new ReportParameter("MaDocGia", p.MaDocGia),
                new ReportParameter("HoTen", p.HoTen),
                new ReportParameter("ThuThu", p.ThuThu)
            });
            rv.RefreshReport();
        }
    }
}
