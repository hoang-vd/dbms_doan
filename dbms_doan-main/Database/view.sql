/*
Views for simplified SELECT logic.
Pattern: stored procedures will SELECT from these views, maintaining EXECUTE-based security.
Run after tables and before granting permissions (or rerun anytime). All views are idempotent via CREATE OR ALTER.
*/
USE BookstoreDB;
GO

/* Roles */
CREATE OR ALTER VIEW dbo.vw_Roles
AS
SELECT r.id,
       r.name,
       r.description,
       r.created_at
FROM dbo.roles r;
GO

/* Employees with role name */
CREATE OR ALTER VIEW dbo.vw_Employees
AS
SELECT e.id,
       e.name,
       e.email,
       e.phone,
       e.address,
       e.birth_date,
       e.role_id,
       r.name AS role_name,
       e.hire_date,
       e.salary,
       e.is_active
FROM dbo.employees e
JOIN dbo.roles r ON r.id = e.role_id;
GO

/* Shifts joined with employee + role */
CREATE OR ALTER VIEW dbo.vw_Shifts
AS
SELECT s.id,
       s.employee_id,
       e.name AS employee_name,
       r.name AS role_name,
       s.shift_date,
       s.start_time,
       s.end_time,
       s.break_duration,
       s.overtime_hours,
       s.status,
       s.notes
FROM dbo.shifts s
JOIN dbo.employees e ON e.id = s.employee_id
JOIN dbo.roles r ON r.id = e.role_id;
GO

/* Users with role + optional employee */
CREATE OR ALTER VIEW dbo.vw_Users
AS
SELECT u.id,
       u.username,
       u.role_id,
       r.name AS role_name,
       u.employee_id,
       e.name AS employee_name,
       u.is_active,
       u.last_login,
       u.created_at,
       u.updated_at
FROM dbo.users u
JOIN dbo.roles r ON r.id = u.role_id
LEFT JOIN dbo.employees e ON e.id = u.employee_id;
GO

PRINT N'Views created/updated successfully.';
