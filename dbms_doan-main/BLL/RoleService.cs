using System;
using System.Collections.Generic;
using BookstoreDBMS.DAL;
using BookstoreDBMS.Models;

namespace BookstoreDBMS.BLL
{
    public class RoleService
    {
        private readonly RoleRepository _roleRepository;

        public RoleService()
        {
            _roleRepository = new RoleRepository();
        }

        public List<Role> GetAllRoles()
        {
            try
            {
                return _roleRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving roles: {ex.Message}");
            }
        }

        public Role GetRoleById(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Role ID must be greater than 0");

                return _roleRepository.GetById(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving role: {ex.Message}");
            }
        }

        public bool AddRole(Role role)
        {
            try
            {
                ValidateRole(role);
                
                var id = _roleRepository.Insert(role);
                return id > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding role: {ex.Message}");
            }
        }

        public bool UpdateRole(Role role)
        {
            try
            {
                ValidateRole(role);
                
                if (role.Id <= 0)
                    throw new ArgumentException("Role ID must be greater than 0");

                return _roleRepository.Update(role);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating role: {ex.Message}");
            }
        }

        public bool DeleteRole(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Role ID must be greater than 0");

                return _roleRepository.Delete(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting role: {ex.Message}");
            }
        }

        private void ValidateRole(Role role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            if (string.IsNullOrWhiteSpace(role.Name))
                throw new ArgumentException("Role name is required");

            if (role.Name.Length > 50)
                throw new ArgumentException("Role name cannot exceed 50 characters");

            if (!string.IsNullOrEmpty(role.Description) && role.Description.Length > 255)
                throw new ArgumentException("Description cannot exceed 255 characters");
        }
    }
}