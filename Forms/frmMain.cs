using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Label = System.Windows.Forms.Label;   // tránh trùng tên với Charting.Label
using QuanLyThuVien.Helpers;
using QuanLyThuVien.Repositories;

namespace QuanLyThuVien.Forms
{
    /// <summary>
    /// Màn hình chính. Menu được dựng theo QUYỀN của nhóm người dùng (RBAC):
    /// chức năng không có quyền sẽ bị ẩn (Visible = false).
    /// </summary>
    public class frmMain : Form
    {
        public bool DangXuat { get; private set; }

        private readonly MenuStrip menu = new MenuStrip();
        private readonly StatusStrip status = new StatusStrip();
        private Chart chart;
        private Series series;

        public frmMain()
        {
            Text = "Quản lý thư viện đại học";
            Font = UI.Fnt;
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(900, 600);

            var lblChao = new Label
            {
                Dock = DockStyle.Top, Height = 70, TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 15f, FontStyle.Bold),
                Text = "Xin chào " + Session.User.HoTen + "  -  Nhóm: " + Session.User.NhomQuyen.TenNhom
            };
            status.Items.Add(new ToolStripStatusLabel("Tài khoản: " + Session.User.TenDangNhap));

            BuildMenu();

            // Biểu đồ chỉ hiện với người có quyền xem báo cáo
            if (Session.Has("BC_DOANHTHU")) { BuildChart(); Controls.Add(chart); }
            Controls.Add(lblChao);
            Controls.Add(status);
            Controls.Add(menu);
            MainMenuStrip = menu;

            Load += (s, e) => LoadChart();
        }

        // ---------------- MENU ----------------
        private void BuildMenu()
        {
            var mHeThong = Root("Hệ thống");
            Item(mHeThong, "Đổi mật khẩu", "DOI_MATKHAU", () => new frmDoiMatKhau());
            Item(mHeThong, "Quản lý người dùng", "QL_USER", () => new frmQuanLyUser());
            Item(mHeThong, "Quản lý nhóm quyền", "QL_NHOMQUYEN", () => new frmNhomQuyen());
            Item(mHeThong, "Quy định thư viện", "QL_QUYDINH", () => new frmQuyDinh());
            var dx = new ToolStripMenuItem("Đăng xuất");
            dx.Click += (s, e) => { DangXuat = true; Close(); };
            var thoat = new ToolStripMenuItem("Thoát");
            thoat.Click += (s, e) => Close();
            mHeThong.DropDownItems.Add(new ToolStripSeparator());
            mHeThong.DropDownItems.Add(dx);
            mHeThong.DropDownItems.Add(thoat);

            var mDanhMuc = Root("Danh mục");
            Item(mDanhMuc, "Sách", "QL_SACH", () => new frmSach());
            Item(mDanhMuc, "Thể loại", "QL_THELOAI", () => new frmTheLoai());
            Item(mDanhMuc, "Tác giả", "QL_TACGIA", () => new frmTacGia());
            Item(mDanhMuc, "Nhà xuất bản", "QL_NXB", () => new frmNhaXuatBan());
            Item(mDanhMuc, "Độc giả", "QL_DOCGIA", () => new frmDocGia());

            var mNghiepVu = Root("Mượn - Trả");
            Item(mNghiepVu, "Mượn sách", "MUON_SACH", () => new frmMuonSach());
            Item(mNghiepVu, "Trả sách / Thu phạt", "TRA_SACH", () => new frmTraSach());
            Item(mNghiepVu, "Danh sách phiếu mượn", "QL_PHIEUMUON", () => new frmDanhSachPhieuMuon());

            var mTraCuu = Root("Tra cứu");
            Item(mTraCuu, "Tra cứu sách", "TRA_CUU_SACH", () => new frmTraCuuSach());
            Item(mTraCuu, "Lịch sử mượn của tôi", "LICH_SU_MUON", () => new frmLichSuMuon());

            var mBaoCao = Root("Báo cáo");
            Item(mBaoCao, "Báo cáo mượn trả & tiền phạt", "BC_DOANHTHU", () => new frmBaoCao());

            // Ẩn menu cha nếu không còn mục con nào hiển thị
            foreach (ToolStripMenuItem root in menu.Items)
                root.Available = root.DropDownItems.Cast<ToolStripItem>().Any(i => i.Available);
        }

        private ToolStripMenuItem Root(string text)
        {
            var m = new ToolStripMenuItem(text);
            menu.Items.Add(m);
            return m;
        }

        private void Item(ToolStripMenuItem root, string text, string quyen, Func<Form> create)
        {
            var it = new ToolStripMenuItem(text) { Available = Session.Has(quyen) };
            it.Click += (s, e) =>
            {
                using (var f = create()) f.ShowDialog(this);
                LoadChart();
            };
            root.DropDownItems.Add(it);
        }

        // ---------------- BIỂU ĐỒ ----------------
        private void BuildChart()
        {
            chart = new Chart { Dock = DockStyle.Fill };
            chart.ChartAreas.Add(new ChartArea("main"));
            chart.Titles.Add(new Title("Top 5 sách được mượn nhiều nhất", Docking.Top, new Font("Segoe UI", 12f, FontStyle.Bold), Color.Black));
            series = new Series("Lượt mượn") { ChartType = SeriesChartType.Column, IsValueShownAsLabel = true };
            chart.Series.Add(series);
        }

        private void LoadChart()
        {
            if (chart == null) return;
            try
            {
                series.Points.Clear();
                foreach (var t in new BaoCaoRepository().TopSachMuon(5))
                    series.Points.AddXY(t.TenSach, t.SoLan);
            }
            catch { /* không làm hỏng màn hình chính nếu biểu đồ lỗi */ }
        }
    }
}
