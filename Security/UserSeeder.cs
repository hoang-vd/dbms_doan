using System;
using System.Data.SqlClient;
using QuanLyNhanVien.Security;

namespace QuanLyNhanVien.Security
{
    public static class UserSeeder
    {
        public static void SeedDefaultUsers(string connectionString)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // Check if admin user has placeholder hash (0x00)
                    string checkSql = "SELECT COUNT(*) FROM users WHERE password_hash = 0x00";
                    using (var cmd = new SqlCommand(checkSql, conn))
                    {
                        int count = (int)cmd.ExecuteScalar();
                        if (count == 0) return; // Already seeded with real hashes
                    }

                    // Update each placeholder user with real hashed password
                    UpdateUserIfPlaceholder(conn, "admin", "Admin@123");
                    UpdateUserIfPlaceholder(conn, "manager", "Manager@123");
                }
            }
            catch
            {
                // Ignore seeding errors silently to avoid blocking app start
            }
        }

        /// <summary>
        /// Ensure there is ALWAYS an admin account with username=admin and password=admin0101.
        /// If the account exists but password differs, it will be reset to admin0101.
        /// If the Admin role does not exist it will be created.
        /// </summary>
        public static void EnsureAdminAccount(string connectionString)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Ensure Admin role exists
                    int adminRoleId;
                    using (var cmdRole = new SqlCommand(@"IF NOT EXISTS(SELECT 1 FROM roles WHERE name = 'Admin')
                                                            BEGIN
                                                                INSERT INTO roles(name, description, created_at) VALUES('Admin','Quản trị hệ thống',GETDATE());
                                                            END
                                                            SELECT id FROM roles WHERE name='Admin';", conn))
                    {
                        adminRoleId = (int)cmdRole.ExecuteScalar();
                    }

                    // Check if admin user exists
                    int? adminUserId = null;
                    byte[] existingHash = null; byte[] existingSalt = null;
                    using (var cmdFind = new SqlCommand("SELECT TOP 1 id, password_hash, password_salt FROM users WHERE username='admin'", conn))
                    using (var reader = cmdFind.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            adminUserId = (int)reader["id"];
                            existingHash = reader["password_hash"] as byte[];
                            existingSalt = reader["password_salt"] as byte[];
                        }
                    }

                    // Desired password
                    string desiredPassword = "admin0101";

                    if (adminUserId == null)
                    {
                        // Create new admin user
                        PasswordHelper.CreatePasswordHash(desiredPassword, out var hash, out var salt);
                        using (var cmdIns = new SqlCommand(@"INSERT INTO users(username,password_hash,password_salt,role_id,is_active,created_at)
                                                             VALUES(@u,@h,@s,@r,1,GETDATE());", conn))
                        {
                            cmdIns.Parameters.AddWithValue("@u", "admin");
                            cmdIns.Parameters.AddWithValue("@h", hash);
                            cmdIns.Parameters.AddWithValue("@s", salt);
                            cmdIns.Parameters.AddWithValue("@r", adminRoleId);
                            cmdIns.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        bool needReset = true;
                        if (existingHash != null && existingSalt != null)
                        {
                            try
                            {
                                if (PasswordHelper.VerifyPassword(desiredPassword, existingHash, existingSalt))
                                    needReset = false; // already matches desired password
                            }
                            catch { /* ignore and reset */ }
                        }
                        if (needReset)
                        {
                            PasswordHelper.CreatePasswordHash(desiredPassword, out var newHash, out var newSalt);
                            using (var cmdUpd = new SqlCommand("UPDATE users SET password_hash=@h,password_salt=@s, role_id=@r, is_active=1 WHERE id=@id", conn))
                            {
                                cmdUpd.Parameters.AddWithValue("@h", newHash);
                                cmdUpd.Parameters.AddWithValue("@s", newSalt);
                                cmdUpd.Parameters.AddWithValue("@r", adminRoleId);
                                cmdUpd.Parameters.AddWithValue("@id", adminUserId.Value);
                                cmdUpd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch
            {
                // Silent fail: do not block application start.
            }
        }

        private static void UpdateUserIfPlaceholder(SqlConnection conn, string username, string plainPassword)
        {
            string sqlSelect = "SELECT id FROM users WHERE username = @u AND password_hash = 0x00";
            using (var cmd = new SqlCommand(sqlSelect, conn))
            {
                cmd.Parameters.AddWithValue("@u", username);
                var obj = cmd.ExecuteScalar();
                if (obj == null) return; // Not existing or already updated
                int id = (int)obj;
                PasswordHelper.CreatePasswordHash(plainPassword, out var hash, out var salt);

                string sqlUpdate = "UPDATE users SET password_hash=@h, password_salt=@s WHERE id=@id";
                using (var up = new SqlCommand(sqlUpdate, conn))
                {
                    up.Parameters.AddWithValue("@h", hash);
                    up.Parameters.AddWithValue("@s", salt);
                    up.Parameters.AddWithValue("@id", id);
                    up.ExecuteNonQuery();
                }
            }
        }
    }
}
