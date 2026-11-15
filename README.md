# Quản lý Thư viện Đại học (Windows Forms, .NET Framework 4.8)

## Cách chạy
1. Mở SQL Server Management Studio, chạy `Database/01_QuanLyThuVien.sql` (tạo DB + dữ liệu mẫu).
2. Mở `QuanLyThuVien.csproj` bằng Visual Studio 2019/2022 (workload ".NET desktop development"). NuGet sẽ tự khôi phục:
   EntityFramework 6.4.4, Microsoft.ReportingServices.ReportViewerControl.Winforms, QRCoder.
3. Nhấn F5. Nếu không kết nối được SQL Server, chương trình tự mở màn hình **Cấu hình server** (lưu vào App.config).

## Tài khoản mẫu
| Tài khoản | Mật khẩu | Nhóm |
|---|---|---|
| admin | admin123 | Admin (toàn quyền) |
| thuthu1 | thuthu123 | Thủ thư |
| docgia1 | docgia123 | Độc giả (gắn thẻ SV001) |

## Cấu trúc
- `Models/` entity EF + DTO · `Data/LibraryContext.cs` · `Helpers/` (SHA-256, Session/RBAC, UI, Config)
- `Repositories/` truy vấn LINQ (Where, Join, GroupBy, Sum). `MuonTraService.cs` chứa **Transaction** mượn/trả.
- `Forms/` các màn hình (giao diện dựng bằng code, không dùng Designer) · `Reports/` 2 file RDLC.

## Đối chiếu yêu cầu đề bài
| Yêu cầu | Nằm ở |
|---|---|
| ≥ 15 Form | 20 form trong `Forms/` (5 form khung + 15 form nghiệp vụ/danh mục/báo cáo) |
| LINQ/EF, tách Repository | `Repositories/*` |
| Băm mật khẩu SHA-256 | `Helpers/SecurityHelper.cs` |
| Phân quyền RBAC (ẩn chức năng) | `Session.cs`, `frmMain.cs`, `frmNhomQuyen.cs` |
| Nghiệp vụ + Transaction | `MuonTraService.LapPhieuMuon / TraSach` |
| RDLC: phiếu mượn, báo cáo lọc ngày | `Reports/*.rdlc`, `frmInPhieuMuon`, `frmBaoCao` |
| Điểm cộng: App.config, biểu đồ, QR | `frmCauHinhServer`, `frmMain` (Chart), `frmTheDocGia` (QR) |
