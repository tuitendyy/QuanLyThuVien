using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyThuVien.Helpers;

namespace QuanLyThuVien.Forms
{
    /// <summary>
    /// Form nền cho các màn hình CRUD: thanh tìm kiếm + vùng nhập liệu + nút Thêm/Sửa/Xóa/Làm mới + lưới dữ liệu.
    /// Form con chỉ cần thêm control vào pnlFields và cài đặt các hàm abstract.
    /// </summary>
    public abstract class CrudForm : Form
    {
        protected readonly Panel pnlSearch = new Panel { Dock = DockStyle.Top, Height = 42 };
        protected readonly Panel pnlFields = new Panel { Dock = DockStyle.Top, Height = 195 };
        protected readonly FlowLayoutPanel pnlButtons = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 46, Padding = new Padding(10, 6, 0, 0) };
        protected readonly DataGridView grid = UI.Grid();
        protected readonly TextBox txtTim = new TextBox { Left = 90, Top = 9, Width = 280 };
        protected readonly Button btnThem = new Button { Text = "Thêm", Width = 90, Height = 30 };
        protected readonly Button btnSua = new Button { Text = "Sửa", Width = 90, Height = 30 };
        protected readonly Button btnXoa = new Button { Text = "Xóa", Width = 90, Height = 30 };
        protected readonly Button btnMoi = new Button { Text = "Làm mới", Width = 90, Height = 30 };

        protected CrudForm(string title)
        {
            Text = title;
            Font = UI.Fnt;
            Size = new Size(1000, 650);
            StartPosition = FormStartPosition.CenterParent;

            var btnTim = UI.Btn("Tìm", 380, 5, 80);
            pnlSearch.Controls.AddRange(new Control[] { UI.Lbl("Tìm kiếm:", 10, 5, 75), txtTim, btnTim });
            pnlButtons.Controls.AddRange(new Control[] { btnThem, btnSua, btnXoa, btnMoi });

            // Thứ tự add + BringToFront để Dock hoạt động đúng: Search / Fields / Buttons / Grid
            Controls.Add(pnlButtons);
            Controls.Add(pnlFields);
            Controls.Add(pnlSearch);
            Controls.Add(grid);
            grid.BringToFront();

            btnTim.Click += (s, e) => Reload();
            txtTim.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { Reload(); e.SuppressKeyPress = true; } };
            btnThem.Click += (s, e) => Exec(DoAdd, "Đã thêm thành công.");
            btnSua.Click += (s, e) => Exec(DoUpdate, "Đã cập nhật thành công.");
            btnXoa.Click += (s, e) => { if (UI.Confirm("Bạn có chắc muốn xóa dòng đang chọn?")) Exec(DoDelete, "Đã xóa."); };
            btnMoi.Click += (s, e) => { ClearFields(); txtTim.Clear(); Reload(); };
            grid.SelectionChanged += (s, e) => { if (grid.CurrentRow != null && grid.CurrentRow.Index >= 0) UI.Run(RowSelected); };
            Load += (s, e) => Reload();
        }

        // ---- Form con cài đặt ----
        protected abstract object GetData(string keyword);
        protected abstract void SetupColumns();
        protected abstract void RowSelected();
        protected abstract void ClearFields();
        protected abstract void DoAdd();
        protected abstract void DoUpdate();
        protected abstract void DoDelete();

        protected void Reload()
        {
            UI.Run(() =>
            {
                grid.DataSource = GetData(txtTim.Text.Trim());
                SetupColumns();
            });
        }

        private void Exec(Action a, string okMessage)
        {
            if (UI.Run(a))
            {
                Reload();
                UI.Info(okMessage);
            }
        }

        /// <summary>Giá trị (chuỗi) của ô trong dòng đang chọn.</summary>
        protected string Cell(string col)
        {
            if (grid.CurrentRow == null) return "";
            var v = grid.CurrentRow.Cells[col].Value;
            return v == null ? "" : v.ToString();
        }

        protected object CellObj(string col)
        {
            return grid.CurrentRow == null ? null : grid.CurrentRow.Cells[col].Value;
        }

        protected void SetDate(DateTimePicker dtp, string col)
        {
            var v = CellObj(col);
            if (v != null && v != DBNull.Value) dtp.Value = Convert.ToDateTime(v);
        }
    }
}
