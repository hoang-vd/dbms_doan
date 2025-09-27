USE BookstoreDB;
GO

/* 1. Create database roles (idempotent) */
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'app_admin')
    CREATE ROLE app_admin;
GO
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'app_manager')
    CREATE ROLE app_manager;
GO

/* 2. (Optional) Create SQL logins and database users for demo */
IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = 'admin_login')
BEGIN
    CREATE LOGIN admin_login WITH PASSWORD = 'AdminLogin@123';
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = 'manager_login')
BEGIN
    CREATE LOGIN manager_login WITH PASSWORD = 'ManagerLogin@123';
END
GO

USE BookstoreDB;
GO
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'admin_login')
    CREATE USER admin_login FOR LOGIN admin_login;
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'manager_login')
    CREATE USER manager_login FOR LOGIN manager_login;
GO

EXEC sp_addrolemember 'app_admin', 'admin_login';
EXEC sp_addrolemember 'app_manager', 'manager_login';
GO

/* 3. Revoke broad permissions (principle of least privilege) */
-- (Only needed if previously granted directly)
-- Example: REVOKE SELECT, INSERT, UPDATE, DELETE ON dbo.employees FROM app_manager; -- defensive

/* 4. Grant EXECUTE on stored procedures */
-- Admin: grant execute on ALL needed procs (can also grant EXECUTE ON SCHEMA::dbo)
GRANT EXECUTE ON OBJECT::sp_GetRoles TO app_admin;
GRANT EXECUTE ON OBJECT::sp_GetRolesForGrid TO app_admin;
GRANT EXECUTE ON OBJECT::sp_InsertRole TO app_admin;
GRANT EXECUTE ON OBJECT::sp_UpdateRole TO app_admin;
GRANT EXECUTE ON OBJECT::sp_DeleteRole TO app_admin;
GRANT EXECUTE ON OBJECT::sp_GetEmployees TO app_admin;
GRANT EXECUTE ON OBJECT::sp_InsertEmployee TO app_admin;
GRANT EXECUTE ON OBJECT::sp_UpdateEmployee TO app_admin;
GRANT EXECUTE ON OBJECT::sp_DeleteEmployee TO app_admin;
GRANT EXECUTE ON OBJECT::sp_GetEmployeesForShift TO app_admin;
GRANT EXECUTE ON OBJECT::sp_GetShiftsByDate TO app_admin;
GRANT EXECUTE ON OBJECT::sp_InsertShift TO app_admin;
GRANT EXECUTE ON OBJECT::sp_UpdateShift TO app_admin;
GRANT EXECUTE ON OBJECT::sp_DeleteShift TO app_admin;
GRANT EXECUTE ON OBJECT::sp_GetUserRoles TO app_admin;
GRANT EXECUTE ON OBJECT::sp_GetActiveEmployees TO app_admin;
GRANT EXECUTE ON OBJECT::sp_GetUsers TO app_admin;
GRANT EXECUTE ON OBJECT::sp_InsertUser TO app_admin;
GRANT EXECUTE ON OBJECT::sp_UpdateUser TO app_admin;
GRANT EXECUTE ON OBJECT::sp_DeleteUser TO app_admin;
GRANT EXECUTE ON OBJECT::sp_UpdateLastLogin TO app_admin;

-- Manager: NO user/role maintenance permissions
GRANT EXECUTE ON OBJECT::sp_GetRoles TO app_manager;                -- for combo
GRANT EXECUTE ON OBJECT::sp_GetRolesForGrid TO app_manager;
GRANT EXECUTE ON OBJECT::sp_InsertRole TO app_manager;
GRANT EXECUTE ON OBJECT::sp_UpdateRole TO app_manager;
GRANT EXECUTE ON OBJECT::sp_DeleteRole TO app_manager;
GRANT EXECUTE ON OBJECT::sp_GetEmployees TO app_manager;
GRANT EXECUTE ON OBJECT::sp_InsertEmployee TO app_manager;
GRANT EXECUTE ON OBJECT::sp_UpdateEmployee TO app_manager;
GRANT EXECUTE ON OBJECT::sp_DeleteEmployee TO app_manager;          -- optional; drop if managers can't delete
GRANT EXECUTE ON OBJECT::sp_GetEmployeesForShift TO app_manager;
GRANT EXECUTE ON OBJECT::sp_GetShiftsByDate TO app_manager;
GRANT EXECUTE ON OBJECT::sp_InsertShift TO app_manager;
GRANT EXECUTE ON OBJECT::sp_UpdateShift TO app_manager;
GRANT EXECUTE ON OBJECT::sp_DeleteShift TO app_manager;
GRANT EXECUTE ON OBJECT::sp_GetActiveEmployees TO app_manager;
GRANT EXECUTE ON OBJECT::sp_UpdateLastLogin TO app_manager;         -- update last login timestamp

/* Do NOT grant: sp_GetRolesForGrid, sp_InsertRole, sp_UpdateRole, sp_DeleteRole, sp_GetUsers, sp_InsertUser, sp_UpdateUser, sp_DeleteUser */

/* 5. Optionally block direct table access (hard protection) */
-- Deny direct DML so they MUST go through procedures (esp. for manager role)
DENY SELECT, INSERT, UPDATE, DELETE ON OBJECT::roles TO app_manager;
DENY SELECT, INSERT, UPDATE, DELETE ON OBJECT::users TO app_manager;
-- Employees & shifts table direct access can also be denied if you only allow SP usage:
DENY SELECT, INSERT, UPDATE, DELETE ON OBJECT::employees TO app_manager;
DENY SELECT, INSERT, UPDATE, DELETE ON OBJECT::shifts TO app_manager;
-- Admin role could still be restricted to SP-only if you prefer (uncomment):
-- DENY SELECT, INSERT, UPDATE, DELETE ON OBJECT::employees TO app_admin;
-- DENY SELECT, INSERT, UPDATE, DELETE ON OBJECT::roles TO app_admin;
-- DENY SELECT, INSERT, UPDATE, DELETE ON OBJECT::users TO app_admin;
-- DENY SELECT, INSERT, UPDATE, DELETE ON OBJECT::shifts TO app_admin;

PRINT N'Grant security script executed successfully.';

SELECT name, is_disabled, create_date FROM sys.server_principals 
WHERE name IN ('admin_login','manager_login');

SELECT name, type_desc FROM sys.database_principals 
WHERE name IN ('admin_login','manager_login');



-- 1. Kiểm tra login tồn tại & trạng thái
SELECT name, is_disabled, default_database_name FROM sys.server_principals
WHERE name IN ('admin_login','manager_login');

-- 2. (Tùy chọn) Reset mật khẩu về giá trị script
ALTER LOGIN admin_login WITH PASSWORD = 'AdminLogin@123';
ALTER LOGIN manager_login WITH PASSWORD = 'ManagerLogin@123';

-- 3. Vào đúng DB
USE BookstoreDB;
GO

-- 4. Kiểm tra user mapping
SELECT name, type_desc FROM sys.database_principals
WHERE name IN ('admin_login','manager_login');

-- 5. Tạo user + gán role nếu thiếu
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name='admin_login')
    CREATE USER admin_login FOR LOGIN admin_login;
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name='manager_login')
    CREATE USER manager_login FOR LOGIN manager_login;

EXEC sp_addrolemember 'app_admin','admin_login';
EXEC sp_addrolemember 'app_manager','manager_login';

-- 6. Test thực thi 1 proc với EXECUTE AS
EXECUTE AS LOGIN = 'admin_login';
EXEC sp_GetEmployees;
REVERT;