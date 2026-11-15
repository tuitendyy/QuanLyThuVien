using System.Linq;
using System.Windows.Forms;
using QuanLyThuVien.Helpers;
using QuanLyThuVien.Models;
using QuanLyThuVien.Repositories;

namespace QuanLyThuVien.Forms
{
    public class frmTacGia : CrudForm
    {
        private readonly CrudRepository<TacGia> repo = new CrudRepository<TacGia>();
        private readonly TextBox txtMa = UI.Txt(130, 10, 300), txtTen = UI.Txt(130, 45, 300), txtQuocTich = UI.Txt(130, 80, 300);

        public frmTacGia() : base("Quản lý tác giả")
        {
            txtMa.ReadOnly = true;
            pnlFields.Height = 130;
            pnlFields.Controls.AddRange(new Control[]
            {
                UI.Lbl("Mã tác giả", 10, 10, 115), txtMa, UI.Lbl("Tên tác giả", 10, 45, 115), txtTen, UI.Lbl("Quốc tịch", 10, 80, 115), txtQuocTich
            });
        }

        protected override object GetData(string kw)
        {
            return repo.Find(x => x.TenTacGia.Contains(kw) || x.QuocTich.Contains(kw))
                       .Select(x => new { x.MaTacGia, x.TenTacGia, x.QuocTich }).ToList();
        }
        protected override void SetupColumns() { UI.Hdr(grid, "MaTacGia", "Mã"); UI.Hdr(grid, "TenTacGia", "Tên tác giả"); UI.Hdr(grid, "QuocTich", "Quốc tịch"); }
        protected override void RowSelected() { txtMa.Text = Cell("MaTacGia"); txtTen.Text = Cell("TenTacGia"); txtQuocTich.Text = Cell("QuocTich"); }
        protected override void ClearFields() { txtMa.Clear(); txtTen.Clear(); txtQuocTich.Clear(); }

        private void Check() { if (string.IsNullOrWhiteSpace(txtTen.Text)) throw new BusinessException("Vui lòng nhập tên tác giả."); }
        protected override void DoAdd() { Check(); repo.Add(new TacGia { TenTacGia = txtTen.Text.Trim(), QuocTich = txtQuocTich.Text.Trim() }); }
        protected override void DoUpdate()
        {
            if (txtMa.Text == "") throw new BusinessException("Hãy chọn một dòng để sửa."); Check();
            repo.Update(new TacGia { MaTacGia = int.Parse(txtMa.Text), TenTacGia = txtTen.Text.Trim(), QuocTich = txtQuocTich.Text.Trim() });
        }
        protected override void DoDelete()
        {
            if (txtMa.Text == "") throw new BusinessException("Hãy chọn một dòng để xóa.");
            repo.Delete(int.Parse(txtMa.Text));
        }
    }
}
