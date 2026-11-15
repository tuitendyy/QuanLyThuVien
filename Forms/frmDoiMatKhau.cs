using System.Drawing;
using System.Windows.Forms;
using QuanLyThuVien.Helpers;
using QuanLyThuVien.Repositories;

namespace QuanLyThuVien.Forms
{
    public class frmDoiMatKhau : Form
    {
        private readonly TextBox txtCu = UI.Txt(160, 25, 230), txtMoi = UI.Txt(160, 65, 230), txtNhapLai = UI.Txt(160, 105, 230);

        public frmDoiMatKhau()
        {
            Text = "Đổi mật khẩu";
            Font = UI.Fnt;
            ClientSize = new Size(420, 210);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false;

            txtCu.UseSystemPasswordChar = txtMoi.UseSystemPasswordChar = txtNhapLai.UseSystemPasswordChar = true;
            var btnOk = UI.Btn("Đổi mật khẩu", 160, 150, 130);
            var btnHuy = UI.Btn("Đóng", 300, 150, 90);
            AcceptButton = btnOk; CancelButton = btnHuy;
            btnHuy.Click += (s, e) => Close();
            btnOk.Click += (s, e) => Doi();

            Controls.AddRange(new Control[]
            {
                UI.Lbl("Mật khẩu cũ:", 20, 25, 135), txtCu,
                UI.Lbl("Mật khẩu mới:", 20, 65, 135), txtMoi,
                UI.Lbl("Nhập lại mật khẩu:", 20, 105, 135), txtNhapLai,
                btnOk, btnHuy
            });
        }

        private void Doi()
        {
            if (txtCu.Text == "" || txtMoi.Text == "") { UI.Warn("Vui lòng nhập đầy đủ thông tin."); return; }
            if (txtMoi.Text.Length < 6) { UI.Warn("Mật khẩu mới phải có ít nhất 6 ký tự."); return; }
            if (txtMoi.Text != txtNhapLai.Text) { UI.Warn("Mật khẩu nhập lại không khớp."); return; }

            if (UI.Run(() => new AuthRepository().DoiMatKhau(Session.User.MaND, txtCu.Text, txtMoi.Text)))
            {
                UI.Info("Đổi mật khẩu thành công.");
                Close();
            }
        }
    }
}
