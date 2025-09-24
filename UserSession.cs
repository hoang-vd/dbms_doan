using System;

namespace QuanLyNhanVien
{
    public class UserSession
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int? EmployeeId { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin => RoleName == "Admin";
        public bool IsManager => RoleName == "Manager";
    }
}
