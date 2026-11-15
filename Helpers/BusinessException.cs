using System;

namespace QuanLyThuVien.Helpers
{
    /// <summary>Lỗi nghiệp vụ (vd: hết sách, thẻ hết hạn...). Message hiển thị thẳng cho người dùng.</summary>
    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message) { }
    }
}
