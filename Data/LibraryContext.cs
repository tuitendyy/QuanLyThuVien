using System.Data.Entity;
using QuanLyThuVien.Helpers;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.Data
{
    /// <summary>
    /// Entity Framework 6 - Code First map vào CSDL có sẵn (tạo bằng file 01_QuanLyThuVien.sql).
    /// Chuỗi kết nối lấy từ App.config (xem ConfigHelper) nên không fix cứng tên máy.
    /// </summary>
    public class LibraryContext : DbContext
    {
        static LibraryContext()
        {
            Database.SetInitializer<LibraryContext>(null);   // không cho EF tự tạo/sửa DB
        }

        public LibraryContext() : base(ConfigHelper.ConnectionString) { }

        public DbSet<NhomQuyen> NhomQuyens { get; set; }
        public DbSet<QuyenHan> QuyenHans { get; set; }
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<TheLoai> TheLoais { get; set; }
        public DbSet<TacGia> TacGias { get; set; }
        public DbSet<NhaXuatBan> NhaXuatBans { get; set; }
        public DbSet<Sach> Sachs { get; set; }
        public DbSet<DocGia> DocGias { get; set; }
        public DbSet<PhieuMuon> PhieuMuons { get; set; }
        public DbSet<ChiTietPhieuMuon> ChiTietPhieuMuons { get; set; }
        public DbSet<QuyDinh> QuyDinhs { get; set; }

        protected override void OnModelCreating(DbModelBuilder mb)
        {
            // NhomQuyen <-> QuyenHan (nhiều-nhiều) qua bảng ChiTietQuyen
            mb.Entity<NhomQuyen>()
              .HasMany(n => n.QuyenHans)
              .WithMany(q => q.NhomQuyens)
              .Map(m => m.ToTable("ChiTietQuyen").MapLeftKey("MaNhom").MapRightKey("MaQuyen"));

            mb.Entity<Sach>().Property(x => x.GiaBia).HasPrecision(18, 0);
            mb.Entity<ChiTietPhieuMuon>().Property(x => x.TienPhat).HasPrecision(18, 0);
        }
    }
}
