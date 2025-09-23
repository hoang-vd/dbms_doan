using System;

namespace BookstoreDBMS.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; } // Optional địa chỉ
        public int RoleId { get; set; }
        public DateTime HireDate { get; set; }
        public decimal? Salary { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation property
        public Role Role { get; set; }

        public Employee()
        {
            HireDate = DateTime.Now.Date;
            IsActive = true;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }
}