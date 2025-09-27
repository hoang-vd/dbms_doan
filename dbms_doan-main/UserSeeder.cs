using System.Data.SqlClient;

namespace QuanLyNhanVien
{
    public static class UserSeeder
    {
        public static void EnsureAdminAccount(string connectionString)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    int adminRoleId;
                    using (var cmdRole = new SqlCommand(@"IF NOT EXISTS(SELECT 1 FROM roles WHERE name = 'Admin')
                                                        BEGIN
                                                            INSERT INTO roles(name, description, created_at) VALUES('Admin','Qu?n tr? h? th?ng',GETDATE());
                                                        END
                                                        SELECT id FROM roles WHERE name='Admin';", conn))
                    {
                        adminRoleId = (int)cmdRole.ExecuteScalar();
                    }

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

                    string desiredPassword = "admin0101";
                    if (adminUserId == null)
                    {
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
                                    needReset = false;
                            }
                            catch { }
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
            catch { }
        }
    }
}
