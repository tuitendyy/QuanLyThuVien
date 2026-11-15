/* =====================================================================
   FILE 01 - SCRIPT TẠO CƠ SỞ DỮ LIỆU: QUẢN LÝ THƯ VIỆN ĐẠI HỌC
   SQL Server 2016+  |  Chạy toàn bộ file trong SSMS
   Mật khẩu băm SHA-256, lưu dạng HEX IN HOA (64 ký tự)
   Tài khoản mẫu:  admin / admin123   thuthu1 / thuthu123   docgia1 / docgia123
   ===================================================================== */
USE master;
GO
IF DB_ID(N'QuanLyThuVien') IS NOT NULL
BEGIN
    ALTER DATABASE QuanLyThuVien SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QuanLyThuVien;
END
GO
CREATE DATABASE QuanLyThuVien COLLATE Vietnamese_CI_AS;
GO
USE QuanLyThuVien;
GO

/* ====================== 1. PHÂN QUYỀN (RBAC) ====================== */
CREATE TABLE NhomQuyen (
    MaNhom      INT IDENTITY(1,1) PRIMARY KEY,
    TenNhom     NVARCHAR(50)  NOT NULL UNIQUE,
    MoTa        NVARCHAR(200) NULL
);

-- Mỗi quyền tương ứng 1 chức năng/nút trên màn hình chính
CREATE TABLE QuyenHan (
    MaQuyen     VARCHAR(30)   PRIMARY KEY,      -- ví dụ: QL_SACH
    TenQuyen    NVARCHAR(100) NOT NULL
);

CREATE TABLE ChiTietQuyen (
    MaNhom      INT         NOT NULL REFERENCES NhomQuyen(MaNhom) ON DELETE CASCADE,
    MaQuyen     VARCHAR(30) NOT NULL REFERENCES QuyenHan(MaQuyen) ON DELETE CASCADE,
    PRIMARY KEY (MaNhom, MaQuyen)
);

CREATE TABLE NguoiDung (
    MaND        INT IDENTITY(1,1) PRIMARY KEY,
    TenDangNhap VARCHAR(50)   NOT NULL UNIQUE,
    MatKhau     VARCHAR(64)   NOT NULL,          -- SHA-256 HEX
    HoTen       NVARCHAR(100) NOT NULL,
    Email       VARCHAR(100)  NULL,
    MaNhom      INT           NOT NULL REFERENCES NhomQuyen(MaNhom),
    MaDocGia    VARCHAR(20)   NULL,                -- liên kết tài khoản độc giả với thẻ độc giả
    HoatDong    BIT           NOT NULL DEFAULT 1,
    NgayTao     DATETIME      NOT NULL DEFAULT GETDATE()
);

/* ====================== 2. DANH MỤC SÁCH ====================== */
CREATE TABLE TheLoai (
    MaTheLoai   INT IDENTITY(1,1) PRIMARY KEY,
    TenTheLoai  NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE TacGia (
    MaTacGia    INT IDENTITY(1,1) PRIMARY KEY,
    TenTacGia   NVARCHAR(100) NOT NULL,
    QuocTich    NVARCHAR(50)  NULL
);

CREATE TABLE NhaXuatBan (
    MaNXB       INT IDENTITY(1,1) PRIMARY KEY,
    TenNXB      NVARCHAR(150) NOT NULL UNIQUE,
    DiaChi      NVARCHAR(200) NULL
);

CREATE TABLE Sach (
    MaSach        INT IDENTITY(1,1) PRIMARY KEY,
    TenSach       NVARCHAR(200) NOT NULL,
    MaTheLoai     INT NOT NULL REFERENCES TheLoai(MaTheLoai),
    MaTacGia      INT NOT NULL REFERENCES TacGia(MaTacGia),
    MaNXB         INT NOT NULL REFERENCES NhaXuatBan(MaNXB),
    NamXuatBan    INT NULL,
    GiaBia        DECIMAL(18,0) NOT NULL DEFAULT 0,
    SoLuongTong   INT NOT NULL CHECK (SoLuongTong >= 0),
    SoLuongCon    INT NOT NULL CHECK (SoLuongCon >= 0),   -- số cuốn đang có trên kệ
    ViTri         NVARCHAR(50) NULL,
    CONSTRAINT CK_Sach_SoLuong CHECK (SoLuongCon <= SoLuongTong)
);

/* ====================== 3. ĐỘC GIẢ ====================== */
CREATE TABLE DocGia (
    MaDocGia    VARCHAR(20)   PRIMARY KEY,        -- mã thẻ / MSSV
    HoTen       NVARCHAR(100) NOT NULL,
    NgaySinh    DATE          NULL,
    GioiTinh    NVARCHAR(5)   NULL,
    Lop         NVARCHAR(50)  NULL,
    Email       VARCHAR(100)  NULL,
    SoDienThoai VARCHAR(15)   NULL,
    NgayCapThe  DATE          NOT NULL DEFAULT GETDATE(),
    NgayHetHan  DATE          NOT NULL,
    TrangThai   BIT           NOT NULL DEFAULT 1   -- 1: thẻ còn hiệu lực
);

ALTER TABLE NguoiDung ADD CONSTRAINT FK_NguoiDung_DocGia FOREIGN KEY (MaDocGia) REFERENCES DocGia(MaDocGia);

/* ====================== 4. MƯỢN - TRẢ - PHẠT ====================== */
CREATE TABLE PhieuMuon (
    MaPhieu     INT IDENTITY(1,1) PRIMARY KEY,
    MaDocGia    VARCHAR(20) NOT NULL REFERENCES DocGia(MaDocGia),
    MaND        INT         NOT NULL REFERENCES NguoiDung(MaND),   -- thủ thư lập phiếu
    NgayMuon    DATETIME    NOT NULL DEFAULT GETDATE(),
    GhiChu      NVARCHAR(200) NULL
);

CREATE TABLE ChiTietPhieuMuon (
    MaPhieu     INT NOT NULL REFERENCES PhieuMuon(MaPhieu) ON DELETE CASCADE,
    MaSach      INT NOT NULL REFERENCES Sach(MaSach),
    NgayHenTra  DATE NOT NULL,
    NgayTra     DATETIME NULL,                    -- NULL = chưa trả
    TienPhat    DECIMAL(18,0) NOT NULL DEFAULT 0,
    DaThuPhat   BIT NOT NULL DEFAULT 0,           -- 0 = còn nợ phạt
    PRIMARY KEY (MaPhieu, MaSach)
);

/* ====================== 5. CẤU HÌNH QUY ĐỊNH ====================== */
CREATE TABLE QuyDinh (
    TenQuyDinh  VARCHAR(50)   PRIMARY KEY,
    GiaTri      INT           NOT NULL,
    MoTa        NVARCHAR(200) NULL
);
GO

/* ====================== INDEX ====================== */
CREATE INDEX IX_Sach_TenSach ON Sach(TenSach);
CREATE INDEX IX_PhieuMuon_DocGia ON PhieuMuon(MaDocGia);
CREATE INDEX IX_CTPM_ChuaTra ON ChiTietPhieuMuon(NgayTra) INCLUDE (MaSach, NgayHenTra);
GO

/* =====================================================================
   DỮ LIỆU MẪU
   ===================================================================== */
-- Quy định
INSERT INTO QuyDinh VALUES
('SoSachToiDa',   5,    N'Số cuốn tối đa một độc giả được mượn cùng lúc'),
('SoNgayMuon',    14,   N'Số ngày được mượn mỗi lượt'),
('PhiPhatMoiNgay',5000, N'Tiền phạt mỗi ngày trả trễ (đồng/cuốn)');

-- Nhóm quyền
INSERT INTO NhomQuyen(TenNhom, MoTa) VALUES
(N'Admin',   N'Toàn quyền hệ thống'),
(N'Thủ thư', N'Quản lý sách, độc giả, mượn trả, báo cáo'),
(N'Độc giả', N'Tra cứu sách, xem lịch sử mượn của mình');

-- Quyền hạn (mỗi quyền = 1 chức năng trên menu)
INSERT INTO QuyenHan VALUES
('QL_USER',      N'Quản lý người dùng'),
('QL_NHOMQUYEN', N'Quản lý nhóm quyền'),
('QL_SACH',      N'Quản lý sách'),
('QL_THELOAI',   N'Quản lý thể loại'),
('QL_TACGIA',    N'Quản lý tác giả'),
('QL_NXB',       N'Quản lý nhà xuất bản'),
('QL_DOCGIA',    N'Quản lý độc giả'),
('MUON_SACH',    N'Lập phiếu mượn sách'),
('TRA_SACH',     N'Trả sách và thu phạt'),
('QL_PHIEUMUON', N'Danh sách phiếu mượn'),
('BC_DOANHTHU',  N'Báo cáo tiền phạt / mượn trả'),
('QL_QUYDINH',   N'Cấu hình quy định thư viện'),
('TRA_CUU_SACH', N'Tra cứu sách'),
('LICH_SU_MUON', N'Xem lịch sử mượn của tôi'),
('DOI_MATKHAU',  N'Đổi mật khẩu');

-- Admin: tất cả quyền
INSERT INTO ChiTietQuyen SELECT 1, MaQuyen FROM QuyenHan;

-- Thủ thư
INSERT INTO ChiTietQuyen VALUES
(2,'QL_SACH'),(2,'QL_THELOAI'),(2,'QL_TACGIA'),(2,'QL_NXB'),(2,'QL_DOCGIA'),
(2,'MUON_SACH'),(2,'TRA_SACH'),(2,'QL_PHIEUMUON'),(2,'BC_DOANHTHU'),
(2,'TRA_CUU_SACH'),(2,'DOI_MATKHAU');

-- Độc giả
INSERT INTO ChiTietQuyen VALUES (3,'TRA_CUU_SACH'),(3,'LICH_SU_MUON'),(3,'DOI_MATKHAU');

-- Người dùng (SHA-256 HEX IN HOA; tính bằng HASHBYTES trên chuỗi varchar = UTF-8/ASCII)
INSERT INTO NguoiDung(TenDangNhap, MatKhau, HoTen, Email, MaNhom) VALUES
('admin',   CONVERT(VARCHAR(64), HASHBYTES('SHA2_256','admin123'),  2), N'Quản trị viên',  'admin@thuvien.edu.vn', 1),
('thuthu1', CONVERT(VARCHAR(64), HASHBYTES('SHA2_256','thuthu123'), 2), N'Nguyễn Thị Thu', 'thu@thuvien.edu.vn',   2),
('docgia1', CONVERT(VARCHAR(64), HASHBYTES('SHA2_256','docgia123'), 2), N'Trần Văn An',     'an@student.edu.vn',    3);

-- Thể loại, tác giả, NXB
INSERT INTO TheLoai(TenTheLoai) VALUES
(N'Công nghệ thông tin'),(N'Văn học'),(N'Kinh tế'),(N'Khoa học tự nhiên'),(N'Ngoại ngữ');

INSERT INTO TacGia(TenTacGia, QuocTich) VALUES
(N'Robert C. Martin', N'Mỹ'),
(N'Nguyễn Nhật Ánh',  N'Việt Nam'),
(N'Nam Cao',          N'Việt Nam'),
(N'Thomas H. Cormen', N'Mỹ'),
(N'N. Gregory Mankiw',N'Mỹ');

INSERT INTO NhaXuatBan(TenNXB, DiaChi) VALUES
(N'NXB Trẻ',            N'TP. Hồ Chí Minh'),
(N'NXB Giáo dục',       N'Hà Nội'),
(N'NXB Khoa học Kỹ thuật', N'Hà Nội'),
(N'NXB Kim Đồng',       N'Hà Nội');

INSERT INTO Sach(TenSach, MaTheLoai, MaTacGia, MaNXB, NamXuatBan, GiaBia, SoLuongTong, SoLuongCon, ViTri) VALUES
(N'Clean Code',                         1, 1, 3, 2019, 250000, 5, 5, N'A1-01'),
(N'Introduction to Algorithms',         1, 4, 3, 2020, 450000, 4, 4, N'A1-02'),
(N'Mắt biếc',                           2, 2, 1, 2018,  95000, 6, 6, N'B2-01'),
(N'Cho tôi xin một vé đi tuổi thơ',     2, 2, 1, 2017,  80000, 6, 6, N'B2-02'),
(N'Chí Phèo',                           2, 3, 2, 2015,  45000, 8, 8, N'B2-03'),
(N'Principles of Economics',            3, 5, 3, 2021, 390000, 3, 3, N'C3-01');

-- Độc giả mẫu (thẻ hết hạn sau 4 năm)
INSERT INTO DocGia(MaDocGia, HoTen, NgaySinh, GioiTinh, Lop, Email, SoDienThoai, NgayCapThe, NgayHetHan) VALUES
('SV001', N'Trần Văn An',    '2004-05-12', N'Nam', N'CNTT01', 'an@student.edu.vn',   '0901000001', GETDATE(), DATEADD(YEAR,4,GETDATE())),
('SV002', N'Lê Thị Bình',    '2004-09-03', N'Nữ',  N'CNTT01', 'binh@student.edu.vn', '0901000002', GETDATE(), DATEADD(YEAR,4,GETDATE())),
('SV003', N'Phạm Minh Châu', '2003-12-20', N'Nữ',  N'KT02',   'chau@student.edu.vn', '0901000003', GETDATE(), DATEADD(YEAR,4,GETDATE()));

-- Gắn tài khoản docgia1 với thẻ SV001
UPDATE NguoiDung SET MaDocGia = 'SV001' WHERE TenDangNhap = 'docgia1';

-- Phiếu mượn mẫu quá hạn 6 ngày (SV002 mượn "Mắt biếc") để demo tính phạt
INSERT INTO PhieuMuon(MaDocGia, MaND, NgayMuon) VALUES ('SV002', 2, DATEADD(DAY,-20,GETDATE()));
INSERT INTO ChiTietPhieuMuon(MaPhieu, MaSach, NgayHenTra) VALUES (SCOPE_IDENTITY(), 3, CAST(DATEADD(DAY,-6,GETDATE()) AS DATE));
UPDATE Sach SET SoLuongCon = SoLuongCon - 1 WHERE MaSach = 3;
GO

/* ====================== KIỂM TRA NHANH ====================== */
SELECT nd.TenDangNhap, nq.TenNhom, nd.MatKhau FROM NguoiDung nd JOIN NhomQuyen nq ON nd.MaNhom = nq.MaNhom;
GO
