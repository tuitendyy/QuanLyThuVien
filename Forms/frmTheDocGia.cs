using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using QRCoder;
using QuanLyThuVien.Helpers;
using QuanLyThuVien.Repositories;

namespace QuanLyThuVien.Forms
{
    /// <summary>Thẻ độc giả có mã QR (điểm cộng). Máy quét mã QR kiểu "bàn phím" sẽ gõ mã thẳng vào ô Mã độc giả khi mượn/trả.</summary>
    public class frmTheDocGia : Form
    {
        private readonly PictureBox pic = new PictureBox { Left = 20, Top = 20, Width = 220, Height = 220, SizeMode = PictureBoxSizeMode.Zoom, BorderStyle = BorderStyle.FixedSingle };

        public frmTheDocGia(string maDocGia)
        {
            Text = "Thẻ độc giả";
            Font = UI.Fnt;
            ClientSize = new Size(520, 270);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false;

            var dg = new DocGiaRepository().Get(maDocGia);
            if (dg == null) { UI.Error("Không tìm thấy độc giả."); Load += (s, e) => Close(); return; }

            using (var gen = new QRCodeGenerator())
            using (var data = gen.CreateQrCode(dg.MaDocGia, QRCodeGenerator.ECCLevel.Q))
            using (var qr = new QRCode(data))
                pic.Image = qr.GetGraphic(8);

            var lbl = new Label
            {
                Left = 260, Top = 20, Width = 240, Height = 170,
                Text = "THẺ ĐỘC GIẢ\n\nMã: " + dg.MaDocGia + "\nHọ tên: " + dg.HoTen + "\nLớp: " + dg.Lop +
                       "\nHết hạn: " + dg.NgayHetHan.ToString("dd/MM/yyyy") + "\nTrạng thái: " + (dg.TrangThai ? "Còn hiệu lực" : "Đã khóa")
            };
            var btnLuu = UI.Btn("Lưu ảnh QR", 260, 205, 130);
            var btnDong = UI.Btn("Đóng", 400, 205, 100);
            btnDong.Click += (s, e) => Close();
            btnLuu.Click += (s, e) =>
            {
                using (var sfd = new SaveFileDialog { Filter = "PNG|*.png", FileName = "QR_" + dg.MaDocGia + ".png" })
                    if (sfd.ShowDialog(this) == DialogResult.OK) UI.Run(() => pic.Image.Save(sfd.FileName, ImageFormat.Png));
            };
            Controls.AddRange(new Control[] { pic, lbl, btnLuu, btnDong });
        }
    }
}
