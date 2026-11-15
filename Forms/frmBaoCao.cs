using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using QuanLyThuVien.Helpers;
using QuanLyThuVien.Repositories;

namespace QuanLyThuVien.Forms
{
    /// <summary>Báo cáo mượn trả và tiền phạt, lọc từ ngày ... đến ngày ... (RDLC).</summary>
    public class frmBaoCao : Form
    {
        private readonly DateTimePicker dtpTu = UI.Dtp(80, 12, 130), dtpDen = UI.Dtp(300, 12, 130);
        private readonly Label lblTong = new Label { Left = 640, Top = 14, Width = 330, Font = UI.FntBold };
        private readonly ReportViewer rv = new ReportViewer { Left = 0, Top = 52, Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };

        public frmBaoCao()
        {
            Text = "Báo cáo mượn trả & tiền phạt";
            Font = UI.Fnt;
            Size = new Size(1100, 720);
            StartPosition = FormStartPosition.CenterParent;

            dtpTu.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpDen.Value = DateTime.Today;
            var btnXem = UI.Btn("Xem báo cáo", 450, 9, 120);
            btnXem.Click += (s, e) => UI.Run(Xem);

            Controls.AddRange(new Control[] { UI.Lbl("Từ ngày:", 10, 12, 65), dtpTu, UI.Lbl("Đến ngày:", 225, 12, 70), dtpDen, btnXem, lblTong, rv });
            rv.Width = ClientSize.Width; rv.Height = ClientSize.Height - 52;
        }

        private void Xem()
        {
            if (dtpTu.Value.Date > dtpDen.Value.Date) throw new BusinessException("'Từ ngày' phải nhỏ hơn hoặc bằng 'Đến ngày'.");
            var rows = new BaoCaoRepository().GetMuonTra(dtpTu.Value, dtpDen.Value);
            lblTong.Text = "Số lượt mượn: " + rows.Count + "   |   Tổng phạt: " + rows.Sum(r => r.TienPhat).ToString("N0") + "đ";

            rv.LocalReport.ReportEmbeddedResource = "QuanLyThuVien.Reports.BaoCaoMuonTra.rdlc";
            rv.LocalReport.DataSources.Clear();
            rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", rows));
            rv.LocalReport.SetParameters(new[]
            {
                new ReportParameter("TuNgay", dtpTu.Value.ToString("dd/MM/yyyy")),
                new ReportParameter("DenNgay", dtpDen.Value.ToString("dd/MM/yyyy")),
                new ReportParameter("NguoiLap", Session.User.HoTen)
            });
            rv.RefreshReport();
        }
    }
}
