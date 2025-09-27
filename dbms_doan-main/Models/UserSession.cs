namespace QuanLyNhanVien.Models
{
    public class UserSession
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int? EmployeeId { get; set; }
        public bool IsActive { get; set; }

        public bool IsAdmin => RoleName != null && RoleName.Equals("Admin", System.StringComparison.OrdinalIgnoreCase);
        public bool IsManager => RoleName != null && RoleName.Equals("Manager", System.StringComparison.OrdinalIgnoreCase);
    }
}
