using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BookstoreDBMS.DAL;
using BookstoreDBMS.Models;

namespace BookstoreDBMS.BLL
{
    public class EmployeeService
    {
        private readonly EmployeeRepository _employeeRepository;

        public EmployeeService()
        {
            _employeeRepository = new EmployeeRepository();
        }

        public List<Employee> GetAllEmployees()
        {
            try
            {
                return _employeeRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving employees: {ex.Message}");
            }
        }

        public Employee GetEmployeeById(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Employee ID must be greater than 0");

                return _employeeRepository.GetById(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving employee: {ex.Message}");
            }
        }

        public bool AddEmployee(Employee employee)
        {
            try
            {
                ValidateEmployee(employee);
                
                var id = _employeeRepository.Insert(employee);
                return id > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding employee: {ex.Message}");
            }
        }

        public bool UpdateEmployee(Employee employee)
        {
            try
            {
                ValidateEmployee(employee);
                
                if (employee.Id <= 0)
                    throw new ArgumentException("Employee ID must be greater than 0");

                return _employeeRepository.Update(employee);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating employee: {ex.Message}");
            }
        }

        public bool DeleteEmployee(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Employee ID must be greater than 0");

                return _employeeRepository.Delete(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting employee: {ex.Message}");
            }
        }

        private void ValidateEmployee(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            if (string.IsNullOrWhiteSpace(employee.Name))
                throw new ArgumentException("Employee name is required");

            if (employee.Name.Length > 100)
                throw new ArgumentException("Employee name cannot exceed 100 characters");

            if (employee.RoleId <= 0)
                throw new ArgumentException("Role ID must be greater than 0");

            if (!string.IsNullOrEmpty(employee.Email))
            {
                if (employee.Email.Length > 100)
                    throw new ArgumentException("Email cannot exceed 100 characters");

                if (!IsValidEmail(employee.Email))
                    throw new ArgumentException("Invalid email format");
            }

            if (!string.IsNullOrEmpty(employee.Phone) && employee.Phone.Length > 20)
                throw new ArgumentException("Phone cannot exceed 20 characters");

            if (employee.Salary.HasValue && employee.Salary < 0)
                throw new ArgumentException("Salary cannot be negative");

            if (employee.HireDate > DateTime.Now.Date)
                throw new ArgumentException("Hire date cannot be in the future");
        }

        private bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }
    }
}