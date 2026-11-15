using System.Drawing;
using System.Windows.Forms;
using QuanLyThuVien.Helpers;
using QuanLyThuVien.Repositories;

namespace QuanLyThuVien.Forms
{
    /// <summary>Cấu hình quy định: số sách tối đa, số ngày mượn, tiền phạt mỗi ngày.</summary>
    public class frmQuyDinh : Form
    {
        private readonly NumericUpDown numSach = new NumericUpDown { Left = 250, Top = 25, Width = 120, Minimum = 1, Maximum = 50 };
        private readonly NumericUpDown numNgay = new NumericUpDown { Left = 250, Top = 65, Width = 120, Minimum = 1, Maximum = 365 };
        private readonly NumericUpDown numPhat = new NumericUpDown { Left = 250, Top = 105, Width = 120, Minimum = 1, Maximum = 1000000, Increment = 1000 };

        public frmQuyDinh()
        {
            Text = "Quy định thư viện";
            Font = UI.Fnt;
            ClientSize = new Size(420, 210);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false;

            var btnLuu = UI.Btn("Lưu", 250, 150, 120);
            btnLuu.Click += (s, e) =>
            {
                var repo = new QuyDinhRepository();
                if (UI.Run(() =>
                {
                    repo.Update(QuyDinhRepository.SoSachToiDa, (int)numSach.Value);
                    repo.Update(QuyDinhRepository.SoNgayMuon, (int)numNgay.Value);
                    repo.Update(QuyDinhRepository.PhiPhatMoiNgay, (int)numPhat.Value);
                })) UI.Info("Đã lưu quy định.");
            };
            Controls.AddRange(new Control[]
            {
                UI.Lbl("Số sách mượn tối đa:", 20, 25, 225), numSach,
                UI.Lbl("Số ngày được mượn:", 20, 65, 225), numNgay,
                UI.Lbl("Phạt mỗi ngày (đồng/cuốn):", 20, 105, 225), numPhat, btnLuu
            });

            Load += (s, e) => UI.Run(() =>
            {
                numSach.Value = QuyDinhRepository.GetValue(QuyDinhRepository.SoSachToiDa);
                numNgay.Value = QuyDinhRepository.GetValue(QuyDinhRepository.SoNgayMuon);
                numPhat.Value = QuyDinhRepository.GetValue(QuyDinhRepository.PhiPhatMoiNgay);
            });
        }
    }
}
