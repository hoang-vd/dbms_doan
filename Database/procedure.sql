USE BookstoreDB;
GO

-- =============================================
-- STORED PROCEDURES FOR EMPLOYEE MANAGEMENT
-- =============================================

-- Get all roles (for dropdown)
CREATE OR ALTER PROCEDURE sp_GetRoles
AS
BEGIN
    SELECT id, name 
    FROM roles 
    WHERE name <> 'Admin' 
    ORDER BY name;
END
GO

-- Get all roles for grid
CREATE OR ALTER PROCEDURE sp_GetRolesForGrid
AS
BEGIN
    SELECT id, name, description, created_at 
    FROM roles 
    ORDER BY name;
END
GO

-- Get all employees
CREATE OR ALTER PROCEDURE sp_GetEmployees
AS
BEGIN
    SELECT 
        e.id, 
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
    FROM employees e 
    INNER JOIN roles r ON e.role_id = r.id
    ORDER BY e.name;
END
GO

-- Insert employee
CREATE OR ALTER PROCEDURE sp_InsertEmployee
    @name NVARCHAR(100),
    @email NVARCHAR(100),
    @phone NVARCHAR(20),
    @address NVARCHAR(255),
    @birth_date DATE,
    @role_id INT,
    @hire_date DATE,
    @salary DECIMAL(12,2),
    @is_active BIT
AS
BEGIN
    INSERT INTO employees (name, email, phone, address, birth_date, role_id, hire_date, salary, is_active) 
    VALUES (@name, @email, @phone, @address, @birth_date, @role_id, @hire_date, @salary, @is_active);
    
    RETURN @@ROWCOUNT;
END
GO

-- Update employee
CREATE OR ALTER PROCEDURE sp_UpdateEmployee
    @id INT,
    @name NVARCHAR(100),
    @email NVARCHAR(100),
    @phone NVARCHAR(20),
    @address NVARCHAR(255),
    @birth_date DATE,
    @role_id INT,
    @hire_date DATE,
    @salary DECIMAL(12,2),
    @is_active BIT
AS
BEGIN
    UPDATE employees 
    SET name = @name, 
        email = @email, 
        phone = @phone, 
        address = @address, 
        birth_date = @birth_date, 
        role_id = @role_id, 
        hire_date = @hire_date, 
        salary = @salary, 
        is_active = @is_active,
        updated_at = GETDATE()
    WHERE id = @id;
    
    RETURN @@ROWCOUNT;
END
GO

-- Delete employee
CREATE OR ALTER PROCEDURE sp_DeleteEmployee
    @id INT
AS
BEGIN
    DELETE FROM employees WHERE id = @id;
    RETURN @@ROWCOUNT;
END
GO

-- =============================================
-- STORED PROCEDURES FOR ROLE MANAGEMENT
-- =============================================

-- Insert role
CREATE OR ALTER PROCEDURE sp_InsertRole
    @name NVARCHAR(50),
    @description NVARCHAR(255)
AS
BEGIN
    INSERT INTO roles (name, description) 
    VALUES (@name, @description);
    
    RETURN @@ROWCOUNT;
END
GO

-- Update role
CREATE OR ALTER PROCEDURE sp_UpdateRole
    @id INT,
    @name NVARCHAR(50),
    @description NVARCHAR(255)
AS
BEGIN
    UPDATE roles 
    SET name = @name, 
        description = @description 
    WHERE id = @id;
    
    RETURN @@ROWCOUNT;
END
GO

-- Delete role
CREATE OR ALTER PROCEDURE sp_DeleteRole
    @id INT
AS
BEGIN
    DELETE FROM roles WHERE id = @id;
    RETURN @@ROWCOUNT;
END
GO

-- =============================================
-- STORED PROCEDURES FOR SHIFT MANAGEMENT
-- =============================================

-- Get employees for shift dropdown
CREATE OR ALTER PROCEDURE sp_GetEmployeesForShift
AS
BEGIN
    SELECT id, name 
    FROM employees 
    ORDER BY name;
END
GO

-- Get shifts by date
CREATE OR ALTER PROCEDURE sp_GetShiftsByDate
    @shift_date DATE
AS
BEGIN
    SELECT 
        s.id, 
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
    FROM shifts s 
    INNER JOIN employees e ON s.employee_id = e.id
    INNER JOIN roles r ON e.role_id = r.id
    WHERE s.shift_date = @shift_date 
    ORDER BY s.start_time;
END
GO

-- Insert shift
CREATE OR ALTER PROCEDURE sp_InsertShift
    @employee_id INT,
    @shift_date DATE,
    @start_time TIME,
    @end_time TIME,
    @break_duration INT,
    @overtime_hours DECIMAL(4,2),
    @notes NVARCHAR(500)
AS
BEGIN
    INSERT INTO shifts (employee_id, shift_date, start_time, end_time, break_duration, overtime_hours, status, notes)
    VALUES (@employee_id, @shift_date, @start_time, @end_time, @break_duration, @overtime_hours, 'Scheduled', @notes);
    
    RETURN @@ROWCOUNT;
END
GO

-- Update shift
CREATE OR ALTER PROCEDURE sp_UpdateShift
    @id INT,
    @employee_id INT,
    @shift_date DATE,
    @start_time TIME,
    @end_time TIME,
    @break_duration INT,
    @overtime_hours DECIMAL(4,2),
    @notes NVARCHAR(500)
AS
BEGIN
    UPDATE shifts 
    SET employee_id = @employee_id, 
        shift_date = @shift_date, 
        start_time = @start_time, 
        end_time = @end_time, 
        break_duration = @break_duration, 
        overtime_hours = @overtime_hours, 
        notes = @notes 
    WHERE id = @id;
    
    RETURN @@ROWCOUNT;
END
GO

-- Delete shift
CREATE OR ALTER PROCEDURE sp_DeleteShift
    @id INT
AS
BEGIN
    DELETE FROM shifts WHERE id = @id;
    RETURN @@ROWCOUNT;
END
GO

-- =============================================
-- STORED PROCEDURES FOR USER MANAGEMENT (ADMIN)
-- =============================================

-- Get roles for user management (Admin/Manager only)
CREATE OR ALTER PROCEDURE sp_GetUserRoles
AS
BEGIN
    SELECT id, name 
    FROM roles 
    WHERE name IN ('Admin', 'Manager')
    ORDER BY name;
END
GO

-- Get active employees for user assignment
CREATE OR ALTER PROCEDURE sp_GetActiveEmployees
AS
BEGIN
    SELECT id, name 
    FROM employees 
    WHERE is_active = 1 
    ORDER BY id;
END
GO

-- Get all users with details
CREATE OR ALTER PROCEDURE sp_GetUsers
AS
BEGIN
    SELECT 
        u.id, 
        u.username, 
        u.role_id, 
        r.name AS role_name, 
        u.employee_id, 
        e.name AS employee_name, 
        u.is_active, 
        u.last_login, 
        u.created_at, 
        u.updated_at
    FROM users u
    INNER JOIN roles r ON u.role_id = r.id
    LEFT JOIN employees e ON u.employee_id = e.id
    ORDER BY u.id DESC;
END
GO

-- Insert user
CREATE OR ALTER PROCEDURE sp_InsertUser
    @username NVARCHAR(50),
    @password_hash VARBINARY(64),
    @password_salt VARBINARY(32),
    @role_id INT,
    @employee_id INT = NULL
AS
BEGIN
    INSERT INTO users (username, password_hash, password_salt, role_id, employee_id, is_active) 
    VALUES (@username, @password_hash, @password_salt, @role_id, @employee_id, 1);
    
    RETURN @@ROWCOUNT;
END
GO

-- Update user
CREATE OR ALTER PROCEDURE sp_UpdateUser
    @id INT,
    @role_id INT,
    @employee_id INT = NULL,
    @password_hash VARBINARY(64) = NULL,
    @password_salt VARBINARY(32) = NULL
AS
BEGIN
    IF @password_hash IS NOT NULL AND @password_salt IS NOT NULL
    BEGIN
        UPDATE users 
        SET role_id = @role_id, 
            employee_id = @employee_id, 
            password_hash = @password_hash, 
            password_salt = @password_salt, 
            updated_at = GETDATE() 
        WHERE id = @id;
    END
    ELSE
    BEGIN
        UPDATE users 
        SET role_id = @role_id, 
            employee_id = @employee_id, 
            updated_at = GETDATE() 
        WHERE id = @id;
    END
    
    RETURN @@ROWCOUNT;
END
GO

-- Delete user
CREATE OR ALTER PROCEDURE sp_DeleteUser
    @id INT
AS
BEGIN
    DELETE FROM users WHERE id = @id;
    RETURN @@ROWCOUNT;
END
GO

-- Update last login time
CREATE OR ALTER PROCEDURE sp_UpdateLastLogin
    @username NVARCHAR(50)
AS
BEGIN
    UPDATE users 
    SET last_login = GETDATE() 
    WHERE username = @username;
END
GO

PRINT N'Tạo Stored Procedures thành công!';
GO