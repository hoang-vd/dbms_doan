using System;
using System.Collections.Generic;
using BookstoreDBMS.DAL;
using BookstoreDBMS.Models;

namespace BookstoreDBMS.BLL
{
    public class SupplierService
    {
        private readonly SupplierRepository _supplierRepository;

        public SupplierService()
        {
            _supplierRepository = new SupplierRepository();
        }

        public List<Supplier> GetAllSuppliers()
        {
            try
            {
                return _supplierRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving suppliers: {ex.Message}");
            }
        }

        public Supplier GetSupplierById(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Supplier ID must be greater than 0");

                return _supplierRepository.GetById(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving supplier: {ex.Message}");
            }
        }

        public bool AddSupplier(Supplier supplier)
        {
            try
            {
                ValidateSupplier(supplier);
                
                var id = _supplierRepository.Insert(supplier);
                return id > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding supplier: {ex.Message}");
            }
        }

        public bool UpdateSupplier(Supplier supplier)
        {
            try
            {
                ValidateSupplier(supplier);
                
                if (supplier.Id <= 0)
                    throw new ArgumentException("Supplier ID must be greater than 0");

                return _supplierRepository.Update(supplier);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating supplier: {ex.Message}");
            }
        }

        public bool DeleteSupplier(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Supplier ID must be greater than 0");

                return _supplierRepository.Delete(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting supplier: {ex.Message}");
            }
        }

        private void ValidateSupplier(Supplier supplier)
        {
            if (supplier == null)
                throw new ArgumentNullException(nameof(supplier));

            if (string.IsNullOrWhiteSpace(supplier.Name))
                throw new ArgumentException("Supplier name is required");

            if (supplier.Name.Length > 200)
                throw new ArgumentException("Supplier name cannot exceed 200 characters");

            if (!string.IsNullOrEmpty(supplier.ContactName) && supplier.ContactName.Length > 100)
                throw new ArgumentException("Contact name cannot exceed 100 characters");

            if (!string.IsNullOrEmpty(supplier.Phone) && supplier.Phone.Length > 20)
                throw new ArgumentException("Phone cannot exceed 20 characters");

            if (!string.IsNullOrEmpty(supplier.Address) && supplier.Address.Length > 500)
                throw new ArgumentException("Address cannot exceed 500 characters");
        }
    }
}