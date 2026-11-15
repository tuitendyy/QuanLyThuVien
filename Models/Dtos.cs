using System;

namespace QuanLyThuVien.Models
{
    // Các lớp "view" dùng để đổ lên DataGridView / báo cáo (kết quả của LINQ Join)

    public class SachDto
    {
        public int MaSach { get; set; }
        public string TenSach { get; set; }
        public int MaTheLoai { get; set; }
        public string TheLoai { get; set; }
        public int MaTacGia { get; set; }
        public string TacGia { get; set; }
        public int MaNXB { get; set; }
        public string NhaXuatBan { get; set; }
        public int? NamXuatBan { get; set; }
        public decimal GiaBia { get; set; }
        public int SoLuongTong { get; set; }
        public int SoLuongCon { get; set; }
        public string ViTri { get; set; }
    }

    public class NguoiDungDto
    {
        public int MaND { get; set; }
        public string TenDangNhap { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public int MaNhom { get; set; }
        public string TenNhom { get; set; }
        public bool HoatDong { get; set; }
        public string MaDocGia { get; set; }
    }

    public class PhieuMuonDto
    {
        public int MaPhieu { get; set; }
        public string MaDocGia { get; set; }
        public string HoTen { get; set; }
        public string ThuThu { get; set; }
        public DateTime NgayMuon { get; set; }
        public int SoSach { get; set; }
    }

    public class ChiTietMuonDto
    {
        public int MaPhieu { get; set; }
        public int MaSach { get; set; }
        public string TenSach { get; set; }
        public string MaDocGia { get; set; }
        public string HoTen { get; set; }
        public DateTime NgayMuon { get; set; }
        public DateTime NgayHenTra { get; set; }
        public DateTime? NgayTra { get; set; }
        public decimal TienPhat { get; set; }
        public bool DaThuPhat { get; set; }
        public string TrangThai { get; set; }   // Đang mượn / Quá hạn / Còn nợ phạt / Đã trả
    }

    public class TopSachDto
    {
        public string TenSach { get; set; }
        public int SoLan { get; set; }
    }

    // Dòng dữ liệu cho RDLC (đã định dạng sẵn thành chuỗi để biểu thức báo cáo đơn giản)
    public class BaoCaoRow
    {
        public int STT { get; set; }
        public int MaPhieu { get; set; }
        public int MaSach { get; set; }
        public string MaDocGia { get; set; }
        public string HoTen { get; set; }
        public string TenSach { get; set; }
        public string NgayMuon { get; set; }
        public string NgayHenTra { get; set; }
        public string NgayTra { get; set; }
        public decimal TienPhat { get; set; }

        public static BaoCaoRow From(ChiTietMuonDto c, int stt)
        {
            return new BaoCaoRow
            {
                STT = stt, MaPhieu = c.MaPhieu, MaSach = c.MaSach, MaDocGia = c.MaDocGia, HoTen = c.HoTen,
                TenSach = c.TenSach,
                NgayMuon = c.NgayMuon.ToString("dd/MM/yyyy"),
                NgayHenTra = c.NgayHenTra.ToString("dd/MM/yyyy"),
                NgayTra = c.NgayTra.HasValue ? c.NgayTra.Value.ToString("dd/MM/yyyy") : "Chưa trả",
                TienPhat = c.TienPhat
            };
        }
    }
}
