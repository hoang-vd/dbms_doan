using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using BookstoreDBMS.Models;

namespace BookstoreDBMS.DAL
{
    public class EmployeeRepository
    {
        public List<Employee> GetAll()
        {
            var employees = new List<Employee>();
            
            using (var connection = DatabaseHelper.GetConnection())
            {
                var query = @"SELECT e.id, e.name, e.phone, e.email, e.address, e.role_id, e.hire_date, 
                            e.salary, e.is_active, e.created_at, e.updated_at, 
                                    r.name as role_name, r.description as role_description
                             FROM employees e
                             INNER JOIN roles r ON e.role_id = r.id
                             ORDER BY e.name";
                
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            employees.Add(new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Name = reader.GetString(reader.GetOrdinal("name")),
                                Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? null : reader.GetString(reader.GetOrdinal("phone")),
                                Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
                                Address = reader.IsDBNull(reader.GetOrdinal("address")) ? null : reader.GetString(reader.GetOrdinal("address")),
                                RoleId = reader.GetInt32(reader.GetOrdinal("role_id")),
                                HireDate = reader.GetDateTime(reader.GetOrdinal("hire_date")),
                                Salary = reader.IsDBNull(reader.GetOrdinal("salary")) ? null : reader.GetDecimal(reader.GetOrdinal("salary")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at")),
                                Role = new Role
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("role_id")),
                                    Name = reader.GetString(reader.GetOrdinal("role_name")),
                                    Description = reader.IsDBNull(reader.GetOrdinal("role_description")) ? null : reader.GetString(reader.GetOrdinal("role_description"))
                                }
                            });
                        }
                    }
                }
            }
            
            return employees;
        }

        public Employee GetById(int id)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                var query = @"SELECT e.id, e.name, e.phone, e.email, e.address, e.role_id, e.hire_date, 
                            e.salary, e.is_active, e.created_at, e.updated_at, 
                                    r.name as role_name, r.description as role_description
                             FROM employees e
                             INNER JOIN roles r ON e.role_id = r.id
                             WHERE e.id = @id";
                
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Name = reader.GetString(reader.GetOrdinal("name")),
                                Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? null : reader.GetString(reader.GetOrdinal("phone")),
                                Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
                                Address = reader.IsDBNull(reader.GetOrdinal("address")) ? null : reader.GetString(reader.GetOrdinal("address")),
                                RoleId = reader.GetInt32(reader.GetOrdinal("role_id")),
                                HireDate = reader.GetDateTime(reader.GetOrdinal("hire_date")),
                                Salary = reader.IsDBNull(reader.GetOrdinal("salary")) ? null : reader.GetDecimal(reader.GetOrdinal("salary")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at")),
                                Role = new Role
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("role_id")),
                                    Name = reader.GetString(reader.GetOrdinal("role_name")),
                                    Description = reader.IsDBNull(reader.GetOrdinal("role_description")) ? null : reader.GetString(reader.GetOrdinal("role_description"))
                                }
                            };
                        }
                    }
                }
            }
            
            return null;
        }

        public int Insert(Employee employee)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
          var query = @"INSERT INTO employees (name, phone, email, address, role_id, hire_date, 
                              salary, is_active, created_at, updated_at) 
                 VALUES (@name, @phone, @email, @address, @role_id, @hire_date, 
                     @salary, @is_active, @created_at, @updated_at); 
                             SELECT SCOPE_IDENTITY();";
                
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", employee.Name);
                    command.Parameters.AddWithValue("@phone", (object)employee.Phone ?? DBNull.Value);
                    command.Parameters.AddWithValue("@email", (object)employee.Email ?? DBNull.Value);
                    command.Parameters.AddWithValue("@address", (object)employee.Address ?? DBNull.Value);
                    command.Parameters.AddWithValue("@role_id", employee.RoleId);
                    command.Parameters.AddWithValue("@hire_date", employee.HireDate);
                    command.Parameters.AddWithValue("@salary", (object)employee.Salary ?? DBNull.Value);
                    command.Parameters.AddWithValue("@is_active", employee.IsActive);
                    command.Parameters.AddWithValue("@created_at", employee.CreatedAt);
                    command.Parameters.AddWithValue("@updated_at", employee.UpdatedAt);
                    
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public bool Update(Employee employee)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                var query = @"UPDATE employees 
                             SET name = @name, phone = @phone, email = @email, address = @address,
                                 role_id = @role_id, hire_date = @hire_date, salary = @salary,
                                 is_active = @is_active, updated_at = @updated_at
                             WHERE id = @id";
                
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", employee.Id);
                    command.Parameters.AddWithValue("@name", employee.Name);
                    command.Parameters.AddWithValue("@phone", (object)employee.Phone ?? DBNull.Value);
                    command.Parameters.AddWithValue("@email", (object)employee.Email ?? DBNull.Value);
                    command.Parameters.AddWithValue("@address", (object)employee.Address ?? DBNull.Value);
                    command.Parameters.AddWithValue("@role_id", employee.RoleId);
                    command.Parameters.AddWithValue("@hire_date", employee.HireDate);
                    command.Parameters.AddWithValue("@salary", (object)employee.Salary ?? DBNull.Value);
                    command.Parameters.AddWithValue("@is_active", employee.IsActive);
                    command.Parameters.AddWithValue("@updated_at", DateTime.Now);
                    
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Delete(int id)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                var query = "DELETE FROM employees WHERE id = @id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}