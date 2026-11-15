using System;
using System.Linq;
using System.Windows.Forms;
using QuanLyThuVien.Helpers;
using QuanLyThuVien.Models;
using QuanLyThuVien.Repositories;

namespace QuanLyThuVien.Forms
{
    public class frmDocGia : CrudForm
    {
        private readonly DocGiaRepository repo = new DocGiaRepository();
        private readonly TextBox txtMa = UI.Txt(120, 10, 240), txtHoTen = UI.Txt(120, 45, 240), txtLop = UI.Txt(120, 150, 240);
        private readonly DateTimePicker dtpNS = UI.Dtp(120, 80, 240), dtpHH = UI.Dtp(530, 80, 240);
        private readonly ComboBox cboGT = UI.Cbo(120, 115, 240);
        private readonly TextBox txtEmail = UI.Txt(530, 10, 240), txtSdt = UI.Txt(530, 45, 240);
        private readonly CheckBox chkTT = new CheckBox { Text = "Thẻ còn hiệu lực", Left = 530, Top = 118, Width = 240, Checked = true };

        public frmDocGia() : base("Quản lý độc giả")
        {
            cboGT.Items.AddRange(new object[] { "Nam", "Nữ", "Khác" });
            cboGT.SelectedIndex = 0;
            dtpNS.Value = new DateTime(2004, 1, 1);
            dtpHH.Value = DateTime.Today.AddYears(4);

            var btnThe = new Button { Text = "Thẻ && QR", Width = 90, Height = 30 };
            btnThe.Click += (s, e) =>
            {
                if (txtMa.Text == "") { UI.Warn("Hãy chọn một độc giả."); return; }
                using (var f = new frmTheDocGia(txtMa.Text)) f.ShowDialog(this);
            };
            pnlButtons.Controls.Add(btnThe);

            pnlFields.Controls.AddRange(new Control[]
            {
                UI.Lbl("Mã độc giả", 10, 10, 105), txtMa, UI.Lbl("Họ tên", 10, 45, 105), txtHoTen,
                UI.Lbl("Ngày sinh", 10, 80, 105), dtpNS, UI.Lbl("Giới tính", 10, 115, 105), cboGT, UI.Lbl("Lớp", 10, 150, 105), txtLop,
                UI.Lbl("Email", 410, 10, 115), txtEmail, UI.Lbl("Số điện thoại", 410, 45, 115), txtSdt,
                UI.Lbl("Hết hạn thẻ", 410, 80, 115), dtpHH, chkTT
            });
        }

        protected override object GetData(string kw)
        {
            return repo.Search(kw).Select(d => new
            {
                d.MaDocGia, d.HoTen, d.NgaySinh, d.GioiTinh, d.Lop, d.Email, d.SoDienThoai, d.NgayCapThe, d.NgayHetHan, d.TrangThai
            }).ToList();
        }

        protected override void SetupColumns()
        {
            UI.Hdr(grid, "MaDocGia", "Mã độc giả"); UI.Hdr(grid, "HoTen", "Họ tên"); UI.Hdr(grid, "NgaySinh", "Ngày sinh", true, "dd/MM/yyyy");
            UI.Hdr(grid, "GioiTinh", "Giới tính"); UI.Hdr(grid, "Lop", "Lớp"); UI.Hdr(grid, "Email", "Email"); UI.Hdr(grid, "SoDienThoai", "SĐT");
            UI.Hdr(grid, "NgayCapThe", "Cấp thẻ", true, "dd/MM/yyyy"); UI.Hdr(grid, "NgayHetHan", "Hết hạn", true, "dd/MM/yyyy"); UI.Hdr(grid, "TrangThai", "Hiệu lực");
        }

        protected override void RowSelected()
        {
            txtMa.Text = Cell("MaDocGia"); txtMa.ReadOnly = true; txtHoTen.Text = Cell("HoTen"); txtLop.Text = Cell("Lop");
            txtEmail.Text = Cell("Email"); txtSdt.Text = Cell("SoDienThoai");
            cboGT.Text = Cell("GioiTinh"); chkTT.Checked = Cell("TrangThai") == "True";
            SetDate(dtpNS, "NgaySinh"); SetDate(dtpHH, "NgayHetHan");
        }

        protected override void ClearFields()
        {
            txtMa.Clear(); txtMa.ReadOnly = false; txtHoTen.Clear(); txtLop.Clear(); txtEmail.Clear(); txtSdt.Clear();
            chkTT.Checked = true; dtpNS.Value = new DateTime(2004, 1, 1); dtpHH.Value = DateTime.Today.AddYears(4);
        }

        private DocGia Read()
        {
            return new DocGia
            {
                MaDocGia = txtMa.Text.Trim(), HoTen = txtHoTen.Text.Trim(), NgaySinh = dtpNS.Value.Date, GioiTinh = cboGT.Text,
                Lop = txtLop.Text.Trim(), Email = txtEmail.Text.Trim(), SoDienThoai = txtSdt.Text.Trim(),
                NgayHetHan = dtpHH.Value.Date, TrangThai = chkTT.Checked
            };
        }

        protected override void DoAdd() { repo.Add(Read()); }
        protected override void DoUpdate() { repo.Update(Read()); }
        protected override void DoDelete()
        {
            if (txtMa.Text == "") throw new BusinessException("Hãy chọn một độc giả để xóa.");
            repo.Delete(txtMa.Text.Trim());
        }
    }
}
