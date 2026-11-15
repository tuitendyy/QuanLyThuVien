using System;
using System.Configuration;
using System.Data.SqlClient;
using QuanLyThuVien.Data;

namespace QuanLyThuVien.Helpers
{
    /// <summary>Đọc/ghi cấu hình kết nối trong App.config (điểm cộng "App.config").</summary>
    public static class ConfigHelper
    {
        public static string Get(string key, string def)
        {
            return ConfigurationManager.AppSettings[key] ?? def;
        }

        public static string ConnectionString
        {
            get
            {
                var b = new SqlConnectionStringBuilder
                {
                    DataSource = Get("Server", @".\SQLEXPRESS"),
                    InitialCatalog = Get("Database", "QuanLyThuVien"),
                    ConnectTimeout = 5
                };
                if (Get("WindowsAuth", "true").ToLower() == "true") b.IntegratedSecurity = true;
                else { b.UserID = Get("User", "sa"); b.Password = Get("Password", ""); }
                return b.ConnectionString;
            }
        }

        public static void Save(string server, string database, bool windowsAuth, string user, string password)
        {
            var cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            Set(cfg, "Server", server);
            Set(cfg, "Database", database);
            Set(cfg, "WindowsAuth", windowsAuth ? "true" : "false");
            Set(cfg, "User", user);
            Set(cfg, "Password", password);
            cfg.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private static void Set(Configuration cfg, string key, string value)
        {
            if (cfg.AppSettings.Settings[key] == null) cfg.AppSettings.Settings.Add(key, value);
            else cfg.AppSettings.Settings[key].Value = value;
        }

        public static bool TestConnection(out string error)
        {
            try
            {
                using (var db = new LibraryContext())
                {
                    db.Database.Connection.Open();
                    db.Database.Connection.Close();
                }
                error = null;
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}
