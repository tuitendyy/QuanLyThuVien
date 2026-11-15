using System.Drawing;
using System.Windows.Forms;
using QuanLyThuVien.Helpers;
using QuanLyThuVien.Repositories;

namespace QuanLyThuVien.Forms
{
    public class frmDangNhap : Form
    {
        private readonly TextBox txtUser = UI.Txt(130, 70, 230), txtPass = UI.Txt(130, 105, 230);

        public frmDangNhap()
        {
            Text = "Đăng nhập - Quản lý thư viện";
            Font = UI.Fnt;
            ClientSize = new Size(400, 230);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false;

            var title = new Label
            {
                Text = "ĐĂNG NHẬP HỆ THỐNG", Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter, Left = 0, Top = 15, Width = 400, Height = 36
            };
            txtPass.UseSystemPasswordChar = true;
            var btnLogin = UI.Btn("Đăng nhập", 130, 150, 110);
            var btnCfg = UI.Btn("Cấu hình server", 250, 150, 130);
            AcceptButton = btnLogin;

            btnLogin.Click += (s, e) => DangNhap();
            btnCfg.Click += (s, e) => { using (var f = new frmCauHinhServer()) f.ShowDialog(this); };

            Controls.AddRange(new Control[]
            {
                title, UI.Lbl("Tên đăng nhập:", 20, 70, 105), txtUser,
                UI.Lbl("Mật khẩu:", 20, 105, 105), txtPass, btnLogin, btnCfg
            });
        }

        private void DangNhap()
        {
            if (txtUser.Text.Trim() == "" || txtPass.Text == "")
            { UI.Warn("Vui lòng nhập tên đăng nhập và mật khẩu."); return; }

            UI.Run(() =>
            {
                var u = new AuthRepository().Login(txtUser.Text.Trim(), txtPass.Text);
                if (u == null) throw new BusinessException("Sai tên đăng nhập hoặc mật khẩu, hoặc tài khoản đã bị khóa.");
                Session.Start(u);
                DialogResult = DialogResult.OK;
            });
        }
    }
}
