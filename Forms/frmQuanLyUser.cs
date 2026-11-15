using System.Windows.Forms;
using QuanLyThuVien.Helpers;
using QuanLyThuVien.Models;
using QuanLyThuVien.Repositories;

namespace QuanLyThuVien.Forms
{
    public class frmQuanLyUser : CrudForm
    {
        private readonly NguoiDungRepository repo = new NguoiDungRepository();
        private readonly TextBox txtMa = UI.Txt(130, 10, 230), txtUser = UI.Txt(130, 45, 230), txtHoTen = UI.Txt(130, 80, 230);
        private readonly TextBox txtEmail = UI.Txt(540, 10, 230), txtPass = UI.Txt(540, 80, 230), txtDocGia = UI.Txt(540, 115, 230);
        private readonly ComboBox cboNhom = UI.Cbo(540, 45, 230);
        private readonly CheckBox chkHoatDong = new CheckBox { Text = "Tài khoản đang hoạt động", Left = 130, Top = 118, Width = 230, Checked = true };

        public frmQuanLyUser() : base("Quản lý người dùng")
        {
            txtMa.ReadOnly = true;
            txtPass.UseSystemPasswordChar = true;
            UI.Bind(cboNhom, new NhomQuyenRepository().GetNhoms(), "TenNhom", "MaNhom");

            pnlFields.Controls.AddRange(new Control[]
            {
                UI.Lbl("Mã ND", 10, 10, 115), txtMa, UI.Lbl("Tên đăng nhập", 10, 45, 115), txtUser,
                UI.Lbl("Họ tên", 10, 80, 115), txtHoTen, chkHoatDong,
                UI.Lbl("Email", 420, 10, 115), txtEmail, UI.Lbl("Nhóm quyền", 420, 45, 115), cboNhom,
                UI.Lbl("Mật khẩu", 420, 80, 115), txtPass, UI.Lbl("Mã độc giả", 420, 115, 115), txtDocGia,
                new Label { Text = "(Khi sửa: để trống mật khẩu = giữ nguyên. Mã độc giả chỉ dùng cho tài khoản độc giả.)", Left = 10, Top = 160, Width = 900, ForeColor = System.Drawing.Color.DimGray }
            });
        }

        protected override object GetData(string kw) { return repo.Search(kw); }

        protected override void SetupColumns()
        {
            UI.Hdr(grid, "MaND", "Mã"); UI.Hdr(grid, "TenDangNhap", "Tên đăng nhập"); UI.Hdr(grid, "HoTen", "Họ tên");
            UI.Hdr(grid, "Email", "Email"); UI.Hdr(grid, "MaNhom", "", false); UI.Hdr(grid, "TenNhom", "Nhóm");
            UI.Hdr(grid, "HoatDong", "Hoạt động"); UI.Hdr(grid, "MaDocGia", "Mã độc giả");
        }

        protected override void RowSelected()
        {
            txtMa.Text = Cell("MaND"); txtUser.Text = Cell("TenDangNhap"); txtHoTen.Text = Cell("HoTen");
            txtEmail.Text = Cell("Email"); txtDocGia.Text = Cell("MaDocGia"); txtPass.Clear();
            chkHoatDong.Checked = Cell("HoatDong") == "True";
            int n; if (int.TryParse(Cell("MaNhom"), out n)) cboNhom.SelectedValue = n;
        }

        protected override void ClearFields()
        {
            txtMa.Clear(); txtUser.Clear(); txtHoTen.Clear(); txtEmail.Clear(); txtPass.Clear(); txtDocGia.Clear();
            chkHoatDong.Checked = true;
        }

        private NguoiDung Read()
        {
            if (cboNhom.SelectedValue == null) throw new BusinessException("Vui lòng chọn nhóm quyền.");
            int id; int.TryParse(txtMa.Text, out id);
            return new NguoiDung
            {
                MaND = id, TenDangNhap = txtUser.Text.Trim(), HoTen = txtHoTen.Text.Trim(), Email = txtEmail.Text.Trim(),
                MaNhom = (int)cboNhom.SelectedValue, HoatDong = chkHoatDong.Checked,
                MaDocGia = string.IsNullOrWhiteSpace(txtDocGia.Text) ? null : txtDocGia.Text.Trim()
            };
        }

        protected override void DoAdd() { var u = Read(); u.MaND = 0; repo.Save(u, txtPass.Text); }

        protected override void DoUpdate()
        {
            if (txtMa.Text == "") throw new BusinessException("Hãy chọn một người dùng trong danh sách.");
            repo.Save(Read(), txtPass.Text);
        }

        protected override void DoDelete()
        {
            if (txtMa.Text == "") throw new BusinessException("Hãy chọn một người dùng trong danh sách.");
            int id = int.Parse(txtMa.Text);
            if (id == Session.User.MaND) throw new BusinessException("Không thể xóa tài khoản đang đăng nhập.");
            repo.Delete(id);
        }
    }
}
