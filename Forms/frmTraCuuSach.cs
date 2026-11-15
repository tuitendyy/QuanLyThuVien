using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuanLyThuVien.Helpers;
using QuanLyThuVien.Models;
using QuanLyThuVien.Repositories;

namespace QuanLyThuVien.Forms
{
    /// <summary>Tra cứu sách (dành cho độc giả và thủ thư).</summary>
    public class frmTraCuuSach : Form
    {
        private readonly TextBox txtTim = UI.Txt(80, 12, 300);
        private readonly ComboBox cboTL = UI.Cbo(480, 12, 200);
        private readonly DataGridView grid = UI.Grid();

        public frmTraCuuSach()
        {
            Text = "Tra cứu sách";
            Font = UI.Fnt;
            ClientSize = new Size(1000, 580);
            StartPosition = FormStartPosition.CenterParent;

            var btnTim = UI.Btn("Tìm", 700, 9, 90);
            grid.Dock = DockStyle.None; grid.SetBounds(10, 55, 980, 515);
            Controls.AddRange(new Control[] { UI.Lbl("Từ khóa:", 10, 12, 65), txtTim, UI.Lbl("Thể loại:", 395, 12, 80), cboTL, btnTim, grid });

            Load += (s, e) =>
            {
                var ds = new List<TheLoai> { new TheLoai { MaTheLoai = 0, TenTheLoai = "-- Tất cả --" } };
                UI.Run(() => ds.AddRange(new CrudRepository<TheLoai>().GetAll()));
                UI.Bind(cboTL, ds, "TenTheLoai", "MaTheLoai");
                Tim();
            };
            btnTim.Click += (s, e) => Tim();
            txtTim.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { Tim(); e.SuppressKeyPress = true; } };
        }

        private void Tim()
        {
            UI.Run(() =>
            {
                int? tl = null;
                if (cboTL.SelectedValue is int && (int)cboTL.SelectedValue > 0) tl = (int)cboTL.SelectedValue;
                grid.DataSource = new SachRepository().Search(txtTim.Text.Trim(), tl, false);
                UI.Hdr(grid, "MaSach", "Mã"); UI.Hdr(grid, "TenSach", "Tên sách"); UI.Hdr(grid, "TheLoai", "Thể loại");
                UI.Hdr(grid, "TacGia", "Tác giả"); UI.Hdr(grid, "NhaXuatBan", "NXB"); UI.Hdr(grid, "NamXuatBan", "Năm");
                UI.Hdr(grid, "SoLuongCon", "Còn lại"); UI.Hdr(grid, "ViTri", "Vị trí kệ");
                foreach (var c in new[] { "MaTheLoai", "MaTacGia", "MaNXB", "GiaBia", "SoLuongTong" }) UI.Hdr(grid, c, "", false);
            });
        }
    }
}
