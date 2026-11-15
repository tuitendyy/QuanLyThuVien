using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyThuVien.Helpers;
using QuanLyThuVien.Models;
using QuanLyThuVien.Repositories;

namespace QuanLyThuVien.Forms
{
    public class frmDanhSachPhieuMuon : Form
    {
        private readonly DateTimePicker dtpTu = UI.Dtp(80, 12, 130), dtpDen = UI.Dtp(300, 12, 130);
        private readonly DataGridView gridPhieu = UI.Grid(), gridCT = UI.Grid();
        private readonly PhieuMuonRepository repo = new PhieuMuonRepository();

        public frmDanhSachPhieuMuon()
        {
            Text = "Danh sách phiếu mượn";
            Font = UI.Fnt;
            ClientSize = new Size(1000, 620);
            StartPosition = FormStartPosition.CenterParent;

            dtpTu.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpDen.Value = DateTime.Today;
            var btnLoc = UI.Btn("Lọc", 450, 9, 80);
            var btnIn = UI.Btn("In phiếu", 540, 9, 100);
            gridPhieu.Dock = DockStyle.None; gridPhieu.SetBounds(10, 50, 980, 280);
            gridCT.Dock = DockStyle.None; gridCT.SetBounds(10, 360, 980, 250);

            Controls.AddRange(new Control[]
            {
                UI.Lbl("Từ ngày:", 10, 12, 65), dtpTu, UI.Lbl("Đến ngày:", 225, 12, 70), dtpDen, btnLoc, btnIn,
                gridPhieu, new Label { Text = "Chi tiết phiếu đang chọn", Left = 10, Top = 337, Width = 300, Font = UI.FntBold }, gridCT
            });

            Load += (s, e) => Loc();
            btnLoc.Click += (s, e) => Loc();
            gridPhieu.SelectionChanged += (s, e) => ChiTiet();
            btnIn.Click += (s, e) =>
            {
                var p = gridPhieu.CurrentRow == null ? null : gridPhieu.CurrentRow.DataBoundItem as PhieuMuonDto;
                if (p == null) { UI.Warn("Hãy chọn một phiếu."); return; }
                using (var f = new frmInPhieuMuon(p.MaPhieu)) f.ShowDialog(this);
            };
        }

        private void Loc()
        {
            if (dtpTu.Value.Date > dtpDen.Value.Date) { UI.Warn("'Từ ngày' phải nhỏ hơn hoặc bằng 'Đến ngày'."); return; }
            UI.Run(() =>
            {
                gridPhieu.DataSource = repo.GetPhieu(dtpTu.Value, dtpDen.Value);
                UI.Hdr(gridPhieu, "MaPhieu", "Mã phiếu"); UI.Hdr(gridPhieu, "MaDocGia", "Mã độc giả"); UI.Hdr(gridPhieu, "HoTen", "Độc giả");
                UI.Hdr(gridPhieu, "ThuThu", "Thủ thư lập"); UI.Hdr(gridPhieu, "NgayMuon", "Ngày mượn", true, "dd/MM/yyyy HH:mm"); UI.Hdr(gridPhieu, "SoSach", "Số sách");
            });
        }

        private void ChiTiet()
        {
            var p = gridPhieu.CurrentRow == null ? null : gridPhieu.CurrentRow.DataBoundItem as PhieuMuonDto;
            if (p == null) { gridCT.DataSource = null; return; }
            UI.Run(() =>
            {
                gridCT.DataSource = repo.GetChiTiet(p.MaPhieu);
                UI.Hdr(gridCT, "MaSach", "Mã sách"); UI.Hdr(gridCT, "TenSach", "Tên sách"); UI.Hdr(gridCT, "NgayHenTra", "Hẹn trả", true, "dd/MM/yyyy");
                UI.Hdr(gridCT, "NgayTra", "Ngày trả", true, "dd/MM/yyyy"); UI.Hdr(gridCT, "TienPhat", "Tiền phạt", true, "N0"); UI.Hdr(gridCT, "TrangThai", "Trạng thái");
                foreach (var c in new[] { "MaPhieu", "MaDocGia", "HoTen", "NgayMuon", "DaThuPhat" }) UI.Hdr(gridCT, c, "", false);
            });
        }
    }
}
