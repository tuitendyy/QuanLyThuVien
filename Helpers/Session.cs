using System;
using System.Collections.Generic;
using System.Linq;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.Helpers
{
    /// <summary>Lưu người dùng đang đăng nhập và tập quyền của nhóm (RBAC).</summary>
    public static class Session
    {
        private static HashSet<string> _quyens = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public static NguoiDung User { get; private set; }

        public static void Start(NguoiDung u)
        {
            User = u;
            _quyens = new HashSet<string>(u.NhomQuyen.QuyenHans.Select(q => q.MaQuyen), StringComparer.OrdinalIgnoreCase);
        }

        public static bool Has(string maQuyen) { return _quyens.Contains(maQuyen); }

        public static void Clear()
        {
            User = null;
            _quyens.Clear();
        }
    }
}
