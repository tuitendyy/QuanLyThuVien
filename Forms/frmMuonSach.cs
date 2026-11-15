using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QuanLyThuVien.Helpers;
using QuanLyThuVien.Models;
using QuanLyThuVien.Repositories;

namespace QuanLyThuVien.Forms
{
    /// <summary>Lập phiếu mượn: nhập mã độc giả (hoặc quét QR) -> chọn sách vào giỏ -> Lập phiếu (Transaction).</summary>
    public class frmMuonSach : Form
    {
        private readonly TextBox txtMa = UI.Txt(110, 15, 150), txtTim = UI.Txt(80, 85, 380), txtGhiChu = UI.Txt(690, 545, 290);
        private readonly Label lblDG = new Label { Left = 380, Top = 12, Width = 600, Height = 55 };
        private readonly DataGridView gridSach = UI.Grid(), gridGio = UI.Grid();
        private readonly BindingList<SachDto> gio = new BindingList<SachDto>();
        private readonly SachRepository sachRepo = new SachRepository();

        public frmMuonSach()
        {
            Text = "Mượn sách";
            Font = UI.Fnt;
            ClientSize = new Size(1000, 600);
            StartPosition = FormStartPosition.CenterParent;

            var btnKT = UI.Btn("Kiểm tra", 270, 12, 100);
            var btnTim = UI.Btn("Tìm", 470, 82, 80);
            var btnThem = UI.Btn("Thêm >>", 580, 250, 95);
            var btnBo = UI.Btn("<< Bỏ", 580, 290, 95);
            var btnLap = UI.Btn("Lập phiếu mượn", 690, 570, 200);

            gridSach.Dock = DockStyle.None; gridSach.SetBounds(10, 120, 560, 410);
            gridGio.Dock = DockStyle.None; gridGio.SetBounds(690, 120, 300, 410);
            gridGio.DataSource = gio;

            Controls.AddRange(new Control[]
            {
                UI.Lbl("Mã độc giả:", 10, 15, 95), txtMa, btnKT, lblDG,
                UI.Lbl("Tìm sách:", 10, 85, 70), txtTim, btnTim,
                new Label { Text = "Sách còn trên kệ", Left = 10, Top = 100, Width = 300, Font = UI.FntBold },
                new Label { Text = "Giỏ mượn", Left = 690, Top = 100, Width = 200, Font = UI.FntBold },
                gridSach, gridGio, btnThem, btnBo,
                UI.Lbl("Ghi chú:", 600, 545, 85), txtGhiChu, btnLap
            });
            txtGhiChu.Top = 540; txtGhiChu.Left = 690; txtGhiChu.Width = 290;

            Load += (s, e) => TimSach();
            txtMa.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { KiemTra(); e.SuppressKeyPress = true; } };   // máy quét QR gõ + Enter
            txtTim.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { TimSach(); e.SuppressKeyPress = true; } };
            btnKT.Click += (s, e) => KiemTra();
            btnTim.Click += (s, e) => TimSach();
            btnThem.Click += (s, e) => ThemVaoGio();
            gridSach.CellDoubleClick += (s, e) => ThemVaoGio();
            btnBo.Click += (s, e) => { var x = gridGio.CurrentRow == null ? null : gridGio.CurrentRow.DataBoundItem as SachDto; if (x != null) gio.Remove(x); };
            btnLap.Click += (s, e) => LapPhieu();
        }

        private void TimSach()
        {
            UI.Run(() =>
            {
                gridSach.DataSource = sachRepo.Search(txtTim.Text.Trim(), null, true);
                UI.Hdr(gridSach, "MaSach", "Mã"); UI.Hdr(gridSach, "TenSach", "Tên sách"); UI.Hdr(gridSach, "TheLoai", "Thể loại");
                UI.Hdr(gridSach, "TacGia", "Tác giả"); UI.Hdr(gridSach, "SoLuongCon", "Còn");
                foreach (var c in new[] { "MaTheLoai", "MaTacGia", "MaNXB", "NhaXuatBan", "NamXuatBan", "GiaBia", "SoLuongTong", "ViTri" })
                    UI.Hdr(gridSach, c, "", false);
                UI.Hdr(gridGio, "MaSach", "Mã"); UI.Hdr(gridGio, "TenSach", "Tên sách");
                foreach (var c in new[] { "MaTheLoai", "TheLoai", "MaTacGia", "TacGia", "MaNXB", "NhaXuatBan", "NamXuatBan", "GiaBia", "SoLuongTong", "SoLuongCon", "ViTri" })
                    UI.Hdr(gridGio, c, "", false);
            });
        }

        private void KiemTra()
        {
            UI.Run(() =>
            {
                string ma = txtMa.Text.Trim();
                var dg = new DocGiaRepository().Get(ma);
                if (dg == null) { lblDG.ForeColor = Color.Firebrick; lblDG.Text = "Không tìm thấy độc giả có mã '" + ma + "'."; return; }
                int dang, qua; decimal no;
                new PhieuMuonRepository().ThongKe(ma, out dang, out no, out qua);
                int max = QuyDinhRepository.GetValue(QuyDinhRepository.SoSachToiDa);
                bool loi = !dg.TrangThai || dg.NgayHetHan.Date < System.DateTime.Today || no > 0 || qua > 0 || dang >= max;
                lblDG.ForeColor = loi ? Color.Firebrick : Color.DarkGreen;
                lblDG.Text = dg.HoTen + " - Lớp " + dg.Lop + "   |   Thẻ hết hạn: " + dg.NgayHetHan.ToString("dd/MM/yyyy") + (dg.TrangThai ? "" : " (ĐÃ KHÓA)") +
                             "\nĐang mượn: " + dang + "/" + max + "   |   Quá hạn: " + qua + "   |   Nợ phạt: " + no.ToString("N0") + "đ";
            });
        }

        private void ThemVaoGio()
        {
            var s = gridSach.CurrentRow == null ? null : gridSach.CurrentRow.DataBoundItem as SachDto;
            if (s == null) return;
            if (gio.Any(x => x.MaSach == s.MaSach)) { UI.Warn("Cuốn này đã có trong giỏ."); return; }
            gio.Add(s);
        }

        private void LapPhieu()
        {
            int maPhieu = 0;
            bool ok = UI.Run(() =>
                maPhieu = new MuonTraService().LapPhieuMuon(txtMa.Text.Trim(), Session.User.MaND, gio.Select(x => x.MaSach).ToList(), txtGhiChu.Text.Trim()));
            if (!ok) return;

            gio.Clear(); txtGhiChu.Clear();
            TimSach(); KiemTra();
            UI.Info("Lập phiếu mượn thành công. Mã phiếu: " + maPhieu);
            using (var f = new frmInPhieuMuon(maPhieu)) f.ShowDialog(this);
        }
    }
}
