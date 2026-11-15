using System.Drawing;
using System.Windows.Forms;
using QuanLyThuVien.Helpers;
using QuanLyThuVien.Models;
using QuanLyThuVien.Repositories;

namespace QuanLyThuVien.Forms
{
    /// <summary>Trả sách và thu tiền phạt. Hiển thị sách đang mượn (kể cả quá hạn) và các khoản phạt còn nợ.</summary>
    public class frmTraSach : Form
    {
        private readonly TextBox txtMa = UI.Txt(110, 15, 160);
        private readonly Label lblDG = new Label { Left = 390, Top = 10, Width = 580, Height = 40 };
        private readonly DataGridView grid = UI.Grid();

        public frmTraSach()
        {
            Text = "Trả sách / Thu phạt";
            Font = UI.Fnt;
            ClientSize = new Size(1000, 560);
            StartPosition = FormStartPosition.CenterParent;

            var btnTim = UI.Btn("Tìm", 280, 12, 90);
            var btnTra = UI.Btn("Trả sách đã chọn", 10, 515, 170);
            var btnThu = UI.Btn("Thu phạt đã chọn", 190, 515, 170);
            grid.Dock = DockStyle.None; grid.SetBounds(10, 60, 980, 440);
            grid.DataBindingComplete += (s, e) => TuMau();

            Controls.AddRange(new Control[] { UI.Lbl("Mã độc giả:", 10, 15, 95), txtMa, btnTim, lblDG, grid, btnTra, btnThu });

            txtMa.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { Tim(); e.SuppressKeyPress = true; } };   // máy quét QR gõ + Enter
            btnTim.Click += (s, e) => Tim();
            btnTra.Click += (s, e) => Tra();
            btnThu.Click += (s, e) => Thu();
        }

        private ChiTietMuonDto Chon() { return grid.CurrentRow == null ? null : grid.CurrentRow.DataBoundItem as ChiTietMuonDto; }

        private void Tim()
        {
            UI.Run(() =>
            {
                string ma = txtMa.Text.Trim();
                var dg = new DocGiaRepository().Get(ma);
                if (dg == null) { lblDG.Text = "Không tìm thấy độc giả."; grid.DataSource = null; return; }
                int dang, qua; decimal no;
                new PhieuMuonRepository().ThongKe(ma, out dang, out no, out qua);
                lblDG.Text = dg.HoTen + " - Lớp " + dg.Lop + "\nĐang mượn: " + dang + "   |   Quá hạn: " + qua + "   |   Nợ phạt đã ghi: " + no.ToString("N0") + "đ";
                grid.DataSource = new PhieuMuonRepository().GetDangMuonVaNo(ma);
                UI.Hdr(grid, "MaPhieu", "Mã phiếu"); UI.Hdr(grid, "MaSach", "Mã sách"); UI.Hdr(grid, "TenSach", "Tên sách");
                UI.Hdr(grid, "MaDocGia", "", false); UI.Hdr(grid, "HoTen", "", false); UI.Hdr(grid, "DaThuPhat", "", false);
                UI.Hdr(grid, "NgayMuon", "Ngày mượn", true, "dd/MM/yyyy"); UI.Hdr(grid, "NgayHenTra", "Hẹn trả", true, "dd/MM/yyyy");
                UI.Hdr(grid, "NgayTra", "Ngày trả", true, "dd/MM/yyyy"); UI.Hdr(grid, "TienPhat", "Tiền phạt", true, "N0"); UI.Hdr(grid, "TrangThai", "Trạng thái");
            });
        }

        private void TuMau()
        {
            foreach (DataGridViewRow r in grid.Rows)
            {
                var d = r.DataBoundItem as ChiTietMuonDto;
                if (d == null) continue;
                if (d.TrangThai == "Quá hạn") r.DefaultCellStyle.BackColor = Color.MistyRose;
                else if (d.TrangThai == "Còn nợ phạt") r.DefaultCellStyle.BackColor = Color.LemonChiffon;
            }
        }

        private void Tra()
        {
            var d = Chon();
            if (d == null || d.NgayTra != null) { UI.Warn("Hãy chọn một cuốn sách đang mượn."); return; }
            decimal phat = 0;
            var svc = new MuonTraService();
            if (!UI.Run(() => phat = svc.TraSach(d.MaPhieu, d.MaSach))) return;

            if (phat > 0)
            {
                if (UI.Confirm("Trả trễ hạn. Tiền phạt: " + phat.ToString("N0") + "đ.\nThu tiền phạt ngay bây giờ?"))
                    UI.Run(() => svc.ThuPhat(d.MaPhieu, d.MaSach));
            }
            else UI.Info("Đã trả sách đúng hạn.");
            Tim();
        }

        private void Thu()
        {
            var d = Chon();
            if (d == null || d.TrangThai != "Còn nợ phạt") { UI.Warn("Hãy chọn dòng có trạng thái 'Còn nợ phạt'."); return; }
            if (UI.Confirm("Xác nhận đã thu " + d.TienPhat.ToString("N0") + "đ tiền phạt?") && UI.Run(() => new MuonTraService().ThuPhat(d.MaPhieu, d.MaSach)))
                Tim();
        }
    }
}
