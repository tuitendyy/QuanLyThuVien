using System.Linq;
using System.Windows.Forms;
using QuanLyThuVien.Helpers;
using QuanLyThuVien.Models;
using QuanLyThuVien.Repositories;

namespace QuanLyThuVien.Forms
{
    public class frmNhaXuatBan : CrudForm
    {
        private readonly CrudRepository<NhaXuatBan> repo = new CrudRepository<NhaXuatBan>();
        private readonly TextBox txtMa = UI.Txt(130, 10, 400), txtTen = UI.Txt(130, 45, 400), txtDiaChi = UI.Txt(130, 80, 400);

        public frmNhaXuatBan() : base("Quản lý nhà xuất bản")
        {
            txtMa.ReadOnly = true;
            pnlFields.Height = 130;
            pnlFields.Controls.AddRange(new Control[]
            {
                UI.Lbl("Mã NXB", 10, 10, 115), txtMa, UI.Lbl("Tên NXB", 10, 45, 115), txtTen, UI.Lbl("Địa chỉ", 10, 80, 115), txtDiaChi
            });
        }

        protected override object GetData(string kw)
        {
            return repo.Find(x => x.TenNXB.Contains(kw) || x.DiaChi.Contains(kw))
                       .Select(x => new { x.MaNXB, x.TenNXB, x.DiaChi }).ToList();
        }
        protected override void SetupColumns() { UI.Hdr(grid, "MaNXB", "Mã"); UI.Hdr(grid, "TenNXB", "Tên nhà xuất bản"); UI.Hdr(grid, "DiaChi", "Địa chỉ"); }
        protected override void RowSelected() { txtMa.Text = Cell("MaNXB"); txtTen.Text = Cell("TenNXB"); txtDiaChi.Text = Cell("DiaChi"); }
        protected override void ClearFields() { txtMa.Clear(); txtTen.Clear(); txtDiaChi.Clear(); }

        private void Check() { if (string.IsNullOrWhiteSpace(txtTen.Text)) throw new BusinessException("Vui lòng nhập tên nhà xuất bản."); }
        protected override void DoAdd() { Check(); repo.Add(new NhaXuatBan { TenNXB = txtTen.Text.Trim(), DiaChi = txtDiaChi.Text.Trim() }); }
        protected override void DoUpdate()
        {
            if (txtMa.Text == "") throw new BusinessException("Hãy chọn một dòng để sửa."); Check();
            repo.Update(new NhaXuatBan { MaNXB = int.Parse(txtMa.Text), TenNXB = txtTen.Text.Trim(), DiaChi = txtDiaChi.Text.Trim() });
        }
        protected override void DoDelete()
        {
            if (txtMa.Text == "") throw new BusinessException("Hãy chọn một dòng để xóa.");
            repo.Delete(int.Parse(txtMa.Text));
        }
    }
}
