using System.Linq;
using System.Windows.Forms;
using QuanLyThuVien.Helpers;
using QuanLyThuVien.Models;
using QuanLyThuVien.Repositories;

namespace QuanLyThuVien.Forms
{
    public class frmTheLoai : CrudForm
    {
        private readonly CrudRepository<TheLoai> repo = new CrudRepository<TheLoai>();
        private readonly TextBox txtMa = UI.Txt(130, 10, 300), txtTen = UI.Txt(130, 45, 300);

        public frmTheLoai() : base("Quản lý thể loại")
        {
            txtMa.ReadOnly = true;
            pnlFields.Height = 95;
            pnlFields.Controls.AddRange(new Control[] { UI.Lbl("Mã thể loại", 10, 10, 115), txtMa, UI.Lbl("Tên thể loại", 10, 45, 115), txtTen });
        }

        protected override object GetData(string kw)
        {
            return repo.Find(x => x.TenTheLoai.Contains(kw)).Select(x => new { x.MaTheLoai, x.TenTheLoai }).ToList();
        }
        protected override void SetupColumns() { UI.Hdr(grid, "MaTheLoai", "Mã"); UI.Hdr(grid, "TenTheLoai", "Tên thể loại"); }
        protected override void RowSelected() { txtMa.Text = Cell("MaTheLoai"); txtTen.Text = Cell("TenTheLoai"); }
        protected override void ClearFields() { txtMa.Clear(); txtTen.Clear(); }

        private void Check() { if (string.IsNullOrWhiteSpace(txtTen.Text)) throw new BusinessException("Vui lòng nhập tên thể loại."); }
        protected override void DoAdd() { Check(); repo.Add(new TheLoai { TenTheLoai = txtTen.Text.Trim() }); }
        protected override void DoUpdate()
        {
            if (txtMa.Text == "") throw new BusinessException("Hãy chọn một dòng để sửa."); Check();
            repo.Update(new TheLoai { MaTheLoai = int.Parse(txtMa.Text), TenTheLoai = txtTen.Text.Trim() });
        }
        protected override void DoDelete()
        {
            if (txtMa.Text == "") throw new BusinessException("Hãy chọn một dòng để xóa.");
            repo.Delete(int.Parse(txtMa.Text));
        }
    }
}
