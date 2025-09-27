using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using BookstoreDBMS.Models;

namespace BookstoreDBMS.DAL
{
    public class RoleRepository
    {
        public List<Role> GetAll()
        {
            var roles = new List<Role>();
            
            using (var connection = DatabaseHelper.GetConnection())
            {
                var query = "SELECT id, name, description, created_at FROM roles ORDER BY name";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            roles.Add(new Role
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Name = reader.GetString(reader.GetOrdinal("name")),
                                Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                            });
                        }
                    }
                }
            }
            
            return roles;
        }

        public Role GetById(int id)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                var query = "SELECT id, name, description, created_at FROM roles WHERE id = @id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Role
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Name = reader.GetString(reader.GetOrdinal("name")),
                                Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                            };
                        }
                    }
                }
            }
            
            return null;
        }

        public int Insert(Role role)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                var query = @"INSERT INTO roles (name, description, created_at) 
                             VALUES (@name, @description, @created_at); 
                             SELECT SCOPE_IDENTITY();";
                
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", role.Name);
                    command.Parameters.AddWithValue("@description", (object)role.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@created_at", role.CreatedAt);
                    
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public bool Update(Role role)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                var query = @"UPDATE roles 
                             SET name = @name, description = @description 
                             WHERE id = @id";
                
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", role.Id);
                    command.Parameters.AddWithValue("@name", role.Name);
                    command.Parameters.AddWithValue("@description", (object)role.Description ?? DBNull.Value);
                    
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Delete(int id)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                var query = "DELETE FROM roles WHERE id = @id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}