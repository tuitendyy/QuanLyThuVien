using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QuanLyThuVien.Helpers;
using QuanLyThuVien.Models;
using QuanLyThuVien.Repositories;

namespace QuanLyThuVien.Forms
{
    /// <summary>Quản lý nhóm quyền: chọn nhóm bên trái, tick các quyền bên phải rồi Lưu.</summary>
    public class frmNhomQuyen : Form
    {
        private readonly NhomQuyenRepository repo = new NhomQuyenRepository();
        private readonly ListBox lstNhom = new ListBox { Left = 15, Top = 40, Width = 240, Height = 360, DisplayMember = "TenNhom" };
        private readonly CheckedListBox clbQuyen = new CheckedListBox { Left = 280, Top = 40, Width = 400, Height = 360, CheckOnClick = true, DisplayMember = "TenQuyen" };
        private readonly TextBox txtTen = UI.Txt(15, 440, 240), txtMoTa = UI.Txt(15, 475, 240);

        public frmNhomQuyen()
        {
            Text = "Quản lý nhóm quyền";
            Font = UI.Fnt;
            ClientSize = new Size(710, 530);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            var btnThem = UI.Btn("Thêm nhóm", 265, 440, 110);
            var btnXoa = UI.Btn("Xóa nhóm", 265, 475, 110);
            var btnLuu = UI.Btn("Lưu quyền của nhóm", 480, 410, 200);

            Controls.AddRange(new Control[]
            {
                new Label { Text = "Nhóm người dùng", Left = 15, Top = 15, Width = 240, Font = UI.FntBold },
                new Label { Text = "Quyền (chức năng được phép dùng)", Left = 280, Top = 15, Width = 400, Font = UI.FntBold },
                lstNhom, clbQuyen,
                new Label { Text = "Tên nhóm mới (dòng dưới: mô tả)", Left = 15, Top = 415, Width = 240 }, txtTen,
                txtMoTa,
                btnThem, btnXoa, btnLuu
            });

            Load += (s, e) => { NapQuyen(); NapNhom(); };
            lstNhom.SelectedIndexChanged += (s, e) => TickQuyen();
            btnThem.Click += (s, e) => { if (UI.Run(() => repo.AddNhom(txtTen.Text, txtMoTa.Text))) { txtTen.Clear(); txtMoTa.Clear(); NapNhom(); } };
            btnXoa.Click += (s, e) =>
            {
                var n = lstNhom.SelectedItem as NhomQuyen;
                if (n == null) return;
                if (UI.Confirm("Xóa nhóm '" + n.TenNhom + "'?") && UI.Run(() => repo.DeleteNhom(n.MaNhom))) NapNhom();
            };
            btnLuu.Click += (s, e) =>
            {
                var n = lstNhom.SelectedItem as NhomQuyen;
                if (n == null) { UI.Warn("Hãy chọn một nhóm."); return; }
                var codes = clbQuyen.CheckedItems.Cast<QuyenHan>().Select(q => q.MaQuyen).ToList();
                if (UI.Run(() => repo.SaveQuyen(n.MaNhom, codes))) UI.Info("Đã lưu quyền. Quyền mới có hiệu lực ở lần đăng nhập sau.");
            };
        }

        private void NapQuyen()
        {
            UI.Run(() => { foreach (var q in repo.GetQuyens()) clbQuyen.Items.Add(q); });
        }

        private void NapNhom()
        {
            UI.Run(() =>
            {
                lstNhom.DataSource = null;
                lstNhom.DisplayMember = "TenNhom";
                lstNhom.DataSource = repo.GetNhoms();
            });
            TickQuyen();
        }

        private void TickQuyen()
        {
            var n = lstNhom.SelectedItem as NhomQuyen;
            if (n == null) return;
            UI.Run(() =>
            {
                List<string> codes = repo.GetMaQuyenCuaNhom(n.MaNhom);
                for (int i = 0; i < clbQuyen.Items.Count; i++)
                    clbQuyen.SetItemChecked(i, codes.Contains(((QuyenHan)clbQuyen.Items[i]).MaQuyen));
            });
        }
    }
}
