using System.Drawing;
using System.Windows.Forms;
using QuanLyThuVien.Helpers;

namespace QuanLyThuVien.Forms
{
    /// <summary>Cấu hình SQL Server (lưu vào App.config) - không fix cứng tên máy trong code.</summary>
    public class frmCauHinhServer : Form
    {
        private readonly TextBox txtServer = UI.Txt(150, 20, 260), txtDb = UI.Txt(150, 55, 260);
        private readonly TextBox txtUser = UI.Txt(150, 130, 260), txtPass = UI.Txt(150, 165, 260);
        private readonly CheckBox chkWin = new CheckBox { Text = "Dùng Windows Authentication", Left = 150, Top = 92, Width = 260 };

        public frmCauHinhServer()
        {
            Text = "Cấu hình kết nối SQL Server";
            Font = UI.Fnt;
            ClientSize = new Size(440, 270);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false;

            txtServer.Text = ConfigHelper.Get("Server", @".\SQLEXPRESS");
            txtDb.Text = ConfigHelper.Get("Database", "QuanLyThuVien");
            chkWin.Checked = ConfigHelper.Get("WindowsAuth", "true").ToLower() == "true";
            txtUser.Text = ConfigHelper.Get("User", "sa");
            txtPass.Text = ConfigHelper.Get("Password", "");
            txtPass.UseSystemPasswordChar = true;
            chkWin.CheckedChanged += (s, e) => { txtUser.Enabled = txtPass.Enabled = !chkWin.Checked; };
            txtUser.Enabled = txtPass.Enabled = !chkWin.Checked;

            var btnOk = UI.Btn("Lưu && kết nối", 150, 215, 130);
            var btnCancel = UI.Btn("Hủy", 290, 215, 120);
            btnCancel.DialogResult = DialogResult.Cancel;
            AcceptButton = btnOk; CancelButton = btnCancel;
            btnOk.Click += (s, e) => LuuVaKetNoi();

            Controls.AddRange(new Control[]
            {
                UI.Lbl("Tên server:", 20, 20, 125), txtServer,
                UI.Lbl("Tên database:", 20, 55, 125), txtDb, chkWin,
                UI.Lbl("Tài khoản SQL:", 20, 130, 125), txtUser,
                UI.Lbl("Mật khẩu:", 20, 165, 125), txtPass,
                btnOk, btnCancel
            });
        }

        private void LuuVaKetNoi()
        {
            if (string.IsNullOrWhiteSpace(txtServer.Text) || string.IsNullOrWhiteSpace(txtDb.Text))
            { UI.Warn("Vui lòng nhập tên server và tên database."); return; }

            if (!UI.Run(() => ConfigHelper.Save(txtServer.Text.Trim(), txtDb.Text.Trim(), chkWin.Checked, txtUser.Text.Trim(), txtPass.Text)))
                return;

            string err;
            if (ConfigHelper.TestConnection(out err))
            {
                UI.Info("Kết nối thành công!");
                DialogResult = DialogResult.OK;
            }
            else UI.Error("Không kết nối được:\n" + err);
        }
    }
}
