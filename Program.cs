using System;
using System.Windows.Forms;
using QuanLyThuVien.Forms;
using QuanLyThuVien.Helpers;

namespace QuanLyThuVien
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Bước 1: chắc chắn kết nối được SQL Server, nếu không thì mở màn hình cấu hình server
            string err;
            while (!ConfigHelper.TestConnection(out err))
            {
                MessageBox.Show("Không kết nối được cơ sở dữ liệu:\n" + err, "Lỗi kết nối",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                using (var f = new frmCauHinhServer())
                    if (f.ShowDialog() != DialogResult.OK) return;
            }

            // Bước 2: vòng lặp Đăng nhập -> Màn hình chính -> Đăng xuất -> Đăng nhập...
            while (true)
            {
                Session.Clear();
                using (var login = new frmDangNhap())
                    if (login.ShowDialog() != DialogResult.OK) return;

                var main = new frmMain();
                Application.Run(main);
                if (!main.DangXuat) return;
            }
        }
    }
}
