using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace QuanLyThuVien.Helpers
{
    /// <summary>Các hàm tiện ích dựng giao diện bằng code và hiển thị thông báo.</summary>
    public static class UI
    {
        public static readonly Font Fnt = new Font("Segoe UI", 10f);
        public static readonly Font FntBold = new Font("Segoe UI", 10f, FontStyle.Bold);

        public static Label Lbl(string text, int x, int y, int w = 105)
        {
            return new Label { Text = text, Left = x, Top = y + 4, Width = w, Height = 24 };
        }

        public static TextBox Txt(int x, int y, int w = 200)
        {
            return new TextBox { Left = x, Top = y, Width = w };
        }

        public static ComboBox Cbo(int x, int y, int w = 200)
        {
            return new ComboBox { Left = x, Top = y, Width = w, DropDownStyle = ComboBoxStyle.DropDownList };
        }

        public static Button Btn(string text, int x, int y, int w = 90)
        {
            return new Button { Text = text, Left = x, Top = y, Width = w, Height = 30 };
        }

        public static DateTimePicker Dtp(int x, int y, int w = 200)
        {
            return new DateTimePicker { Left = x, Top = y, Width = w, Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy" };
        }

        public static DataGridView Grid()
        {
            var g = new DataGridView
            {
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill
            };
            g.ColumnHeadersDefaultCellStyle.Font = FntBold;
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 236, 245);
            return g;
        }

        /// <summary>Đặt tiêu đề cột / ẩn cột / định dạng cho DataGridView.</summary>
        public static void Hdr(DataGridView g, string col, string text, bool visible = true, string fmt = null)
        {
            var c = g.Columns[col];
            if (c == null) return;
            c.HeaderText = text;
            c.Visible = visible;
            if (fmt != null) c.DefaultCellStyle.Format = fmt;
        }

        public static void Bind(ComboBox c, object data, string display, string value)
        {
            c.DataSource = data;
            c.DisplayMember = display;
            c.ValueMember = value;
        }

        public static int ParseInt(string s, string field)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0;
            int v;
            if (!int.TryParse(s.Trim(), out v) || v < 0) throw new BusinessException(field + " phải là số nguyên không âm.");
            return v;
        }

        public static decimal ParseDecimal(string s, string field)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0;
            decimal v;
            if (!decimal.TryParse(s.Trim(), out v) || v < 0) throw new BusinessException(field + " phải là số không âm.");
            return v;
        }

        public static void Info(string m) { MessageBox.Show(m, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        public static void Warn(string m) { MessageBox.Show(m, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        public static void Error(string m) { MessageBox.Show(m, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        public static bool Confirm(string m)
        {
            return MessageBox.Show(m, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        /// <summary>Chạy một hành động, bắt mọi lỗi và hiện thông báo thân thiện. Trả về true nếu thành công.</summary>
        public static bool Run(Action a)
        {
            try { a(); return true; }
            catch (Exception ex) { Error(Msg(ex)); return false; }
        }

        public static string Msg(Exception ex)
        {
            if (ex is BusinessException) return ex.Message;
            var v = ex as DbEntityValidationException;
            if (v != null)
                return string.Join("\n", v.EntityValidationErrors.SelectMany(e => e.ValidationErrors).Select(e => e.ErrorMessage));
            Exception inner = ex;
            while (inner.InnerException != null) inner = inner.InnerException;
            if (ex is DbUpdateException)
                return "Không thể lưu dữ liệu (dữ liệu đang được sử dụng ở nơi khác hoặc vi phạm ràng buộc).\nChi tiết: " + inner.Message;
            return inner.Message;
        }
    }
}
