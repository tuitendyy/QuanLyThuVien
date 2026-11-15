using System.Drawing;
using System.Windows.Forms;
using QuanLyThuVien.Helpers;
using QuanLyThuVien.Repositories;

namespace QuanLyThuVien.Forms
{
    /// <summary>Lịch sử mượn. Tài khoản độc giả chỉ xem được của chính mình; Admin nhập mã độc giả bất kỳ.</summary>
    public class frmLichSuMuon : Form
    {
        private readonly TextBox txtMa = UI.Txt(110, 12, 160);
        private readonly DataGridView grid = UI.Grid();

        public frmLichSuMuon()
        {
            Text = "Lịch sử mượn sách";
            Font = UI.Fnt;
            ClientSize = new Size(1000, 560);
            StartPosition = FormStartPosition.CenterParent;

            var btnXem = UI.Btn("Xem", 280, 9, 90);
            grid.Dock = DockStyle.None; grid.SetBounds(10, 55, 980, 495);
            Controls.AddRange(new Control[] { UI.Lbl("Mã độc giả:", 10, 12, 95), txtMa, btnXem, grid });

            string maCuaToi = Session.User.MaDocGia;
            if (!string.IsNullOrEmpty(maCuaToi)) { txtMa.Text = maCuaToi; txtMa.ReadOnly = true; }   // độc giả chỉ xem của mình

            Load += (s, e) => { if (txtMa.Text != "") Xem(); };
            btnXem.Click += (s, e) => Xem();
        }

        private void Xem()
        {
            UI.Run(() =>
            {
                string ma = string.IsNullOrEmpty(Session.User.MaDocGia) ? txtMa.Text.Trim() : Session.User.MaDocGia;
                grid.DataSource = new PhieuMuonRepository().GetLichSu(ma);
                UI.Hdr(grid, "MaPhieu", "Mã phiếu"); UI.Hdr(grid, "MaSach", "", false); UI.Hdr(grid, "TenSach", "Tên sách");
                UI.Hdr(grid, "MaDocGia", "", false); UI.Hdr(grid, "HoTen", "", false); UI.Hdr(grid, "DaThuPhat", "", false);
                UI.Hdr(grid, "NgayMuon", "Ngày mượn", true, "dd/MM/yyyy"); UI.Hdr(grid, "NgayHenTra", "Hẹn trả", true, "dd/MM/yyyy");
                UI.Hdr(grid, "NgayTra", "Ngày trả", true, "dd/MM/yyyy"); UI.Hdr(grid, "TienPhat", "Tiền phạt", true, "N0"); UI.Hdr(grid, "TrangThai", "Trạng thái");
            });
        }
    }
}
