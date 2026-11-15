using System.Security.Cryptography;
using System.Text;

namespace QuanLyThuVien.Helpers
{
    public static class SecurityHelper
    {
        /// <summary>
        /// Băm mật khẩu bằng SHA-256, trả về chuỗi HEX IN HOA 64 ký tự.
        /// Kết quả khớp với HASHBYTES('SHA2_256', ...) trong file SQL.
        /// </summary>
        public static string Hash(string plain)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(plain ?? ""));
                var sb = new StringBuilder(64);
                foreach (byte b in bytes) sb.Append(b.ToString("X2"));
                return sb.ToString();
            }
        }
    }
}
