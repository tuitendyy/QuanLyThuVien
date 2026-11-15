using System.Windows.Forms;
using QuanLyThuVien.Helpers;
using QuanLyThuVien.Models;
using QuanLyThuVien.Repositories;

namespace QuanLyThuVien.Forms
{
    public class frmSach : CrudForm
    {
        private readonly SachRepository repo = new SachRepository();
        private readonly TextBox txtMa = UI.Txt(110, 10, 260), txtTen = UI.Txt(110, 45, 260);
        private readonly ComboBox cboTL = UI.Cbo(110, 80, 260), cboTG = UI.Cbo(110, 115, 260), cboNXB = UI.Cbo(110, 150, 260);
        private readonly TextBox txtNam = UI.Txt(530, 10, 240), txtGia = UI.Txt(530, 45, 240), txtTong = UI.Txt(530, 80, 240);
        private readonly TextBox txtCon = UI.Txt(530, 115, 240), txtViTri = UI.Txt(530, 150, 240);

        public frmSach() : base("Quản lý sách")
        {
            txtMa.ReadOnly = true; txtCon.ReadOnly = true;
            UI.Bind(cboTL, new CrudRepository<TheLoai>().GetAll(), "TenTheLoai", "MaTheLoai");
            UI.Bind(cboTG, new CrudRepository<TacGia>().GetAll(), "TenTacGia", "MaTacGia");
            UI.Bind(cboNXB, new CrudRepository<NhaXuatBan>().GetAll(), "TenNXB", "MaNXB");

            pnlFields.Controls.AddRange(new Control[]
            {
                UI.Lbl("Mã sách", 10, 10, 95), txtMa, UI.Lbl("Tên sách", 10, 45, 95), txtTen,
                UI.Lbl("Thể loại", 10, 80, 95), cboTL, UI.Lbl("Tác giả", 10, 115, 95), cboTG, UI.Lbl("NXB", 10, 150, 95), cboNXB,
                UI.Lbl("Năm XB", 410, 10, 115), txtNam, UI.Lbl("Giá bìa", 410, 45, 115), txtGia,
                UI.Lbl("Tổng số lượng", 410, 80, 115), txtTong, UI.Lbl("Còn trên kệ", 410, 115, 115), txtCon,
                UI.Lbl("Vị trí kệ", 410, 150, 115), txtViTri
            });
        }

        protected override object GetData(string kw) { return repo.Search(kw); }

        protected override void SetupColumns()
        {
            UI.Hdr(grid, "MaSach", "Mã"); UI.Hdr(grid, "TenSach", "Tên sách"); UI.Hdr(grid, "MaTheLoai", "", false);
            UI.Hdr(grid, "TheLoai", "Thể loại"); UI.Hdr(grid, "MaTacGia", "", false); UI.Hdr(grid, "TacGia", "Tác giả");
            UI.Hdr(grid, "MaNXB", "", false); UI.Hdr(grid, "NhaXuatBan", "NXB"); UI.Hdr(grid, "NamXuatBan", "Năm");
            UI.Hdr(grid, "GiaBia", "Giá bìa", true, "N0"); UI.Hdr(grid, "SoLuongTong", "Tổng"); UI.Hdr(grid, "SoLuongCon", "Còn"); UI.Hdr(grid, "ViTri", "Vị trí");
        }

        protected override void RowSelected()
        {
            txtMa.Text = Cell("MaSach"); txtTen.Text = Cell("TenSach"); txtNam.Text = Cell("NamXuatBan");
            txtGia.Text = Cell("GiaBia"); txtTong.Text = Cell("SoLuongTong"); txtCon.Text = Cell("SoLuongCon"); txtViTri.Text = Cell("ViTri");
            int v;
            if (int.TryParse(Cell("MaTheLoai"), out v)) cboTL.SelectedValue = v;
            if (int.TryParse(Cell("MaTacGia"), out v)) cboTG.SelectedValue = v;
            if (int.TryParse(Cell("MaNXB"), out v)) cboNXB.SelectedValue = v;
        }

        protected override void ClearFields()
        {
            txtMa.Clear(); txtTen.Clear(); txtNam.Clear(); txtGia.Clear(); txtTong.Clear(); txtCon.Clear(); txtViTri.Clear();
        }

        private Sach Read()
        {
            if (cboTL.SelectedValue == null || cboTG.SelectedValue == null || cboNXB.SelectedValue == null)
                throw new BusinessException("Vui lòng chọn thể loại, tác giả và nhà xuất bản (hãy thêm vào danh mục trước nếu chưa có).");
            int id; int.TryParse(txtMa.Text, out id);
            return new Sach
            {
                MaSach = id, TenSach = txtTen.Text.Trim(),
                MaTheLoai = (int)cboTL.SelectedValue, MaTacGia = (int)cboTG.SelectedValue, MaNXB = (int)cboNXB.SelectedValue,
                NamXuatBan = txtNam.Text.Trim() == "" ? (int?)null : UI.ParseInt(txtNam.Text, "Năm xuất bản"),
                GiaBia = UI.ParseDecimal(txtGia.Text, "Giá bìa"), SoLuongTong = UI.ParseInt(txtTong.Text, "Tổng số lượng"),
                ViTri = txtViTri.Text.Trim()
            };
        }

        protected override void DoAdd() { var s = Read(); s.MaSach = 0; repo.Save(s); }
        protected override void DoUpdate()
        {
            if (txtMa.Text == "") throw new BusinessException("Hãy chọn một cuốn sách để sửa.");
            repo.Save(Read());
        }
        protected override void DoDelete()
        {
            if (txtMa.Text == "") throw new BusinessException("Hãy chọn một cuốn sách để xóa.");
            repo.Delete(int.Parse(txtMa.Text));
        }
    }
}
