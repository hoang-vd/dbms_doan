using QuanLyNhanVien;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace QuanLyNhanVien
{
    // Form đăng nhập
    public partial class FormLogin : Form
    {
        private readonly string _connectionString = @"Server=GYEST\SQLEXPRESS;Database=BookstoreDB;Trusted_Connection=True;TrustServerCertificate=True;";
        private readonly string _adminSqlLoginCnn = @"Server=GYEST\SQLEXPRESS;Database=BookstoreDB;User Id=admin_login;Password=AdminLogin@123;TrustServerCertificate=True;";
        private readonly string _managerSqlLoginCnn = @"Server=GYEST\SQLEXPRESS;Database=BookstoreDB;User Id=manager_login;Password=ManagerLogin@123;TrustServerCertificate=True;";

        public UserSession Session { get; private set; }
        public string EffectiveConnectionString { get; private set; } // Connection string to use for Form1 (DB enforced security)

        public FormLogin()
        {
            try { UserSeeder.EnsureAdminAccount(_connectionString); } catch { }
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            AttemptLogin();
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AttemptLogin();
            }
        }

        private void AttemptLogin()
        {
            var username = txtUsername.Text.Trim();
            var password = txtPassword.Text;
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập và mật khẩu", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string sql = @"SELECT TOP 1 u.id, u.username, u.password_hash, u.password_salt, u.role_id, u.employee_id, u.is_active, r.name role_name
                                     FROM users u
                                     JOIN roles r ON r.id = u.role_id
                                     WHERE u.username = @username";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu", "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            if (!reader.GetBoolean(reader.GetOrdinal("is_active")))
                            {
                                MessageBox.Show("Tài khoản đã bị khóa", "Không thể đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            byte[] hash = (byte[])reader["password_hash"];
                            byte[] salt = (byte[])reader["password_salt"];
                            if (!PasswordHelper.VerifyPassword(password, hash, salt))
                            {
                                MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu", "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            Session = new UserSession
                            {
                                UserId = (int)reader["id"],
                                Username = reader["username"].ToString(),
                                RoleId = (int)reader["role_id"],
                                RoleName = reader["role_name"].ToString(),
                                EmployeeId = reader["employee_id"] == DBNull.Value ? (int?)null : (int)reader["employee_id"],
                                IsActive = true
                            };
                        }
                    }
                    if (Session != null)
                    {
                        using (var update = new SqlCommand("sp_UpdateLastLogin", conn))
                        {
                            update.CommandType = System.Data.CommandType.StoredProcedure;
                            update.Parameters.AddWithValue("@username", Session.Username);
                            update.ExecuteNonQuery();
                        }
                    }
                }

                if (Session != null)
                {
                    // Choose connection string based on role (DB GRANT enforcement)
                    EffectiveConnectionString = Session.IsAdmin ? _adminSqlLoginCnn : _managerSqlLoginCnn;
                    // Test secondary connection to ensure GRANT script executed
                    try
                    {
                        using (var roleConn = new SqlConnection(EffectiveConnectionString))
                        {
                            roleConn.Open();
                            // simple sanity check: try execute a common proc
                            using (var testCmd = new SqlCommand(Session.IsAdmin ? "sp_GetEmployees" : "sp_GetEmployees", roleConn))
                            {
                                testCmd.CommandType = System.Data.CommandType.StoredProcedure;
                                testCmd.ExecuteNonQuery();
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("Kết nối bằng SQL login theo vai trò thất bại. Hãy chạy script grant.sql trước.\nChi tiết: " + ex.Message, "Lỗi phân quyền DB", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Session = null;
                        return;
                    }

                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Lỗi SQL: {ex.Number} - {ex.Message}", "Lỗi cơ sở dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }
    }
}

