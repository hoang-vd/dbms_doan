-- kết hợp insert update user ko cần dùng alter thêm
USE BookstoreDB;
GO

-- =============================================
-- STORED PROCEDURES FOR EMPLOYEE MANAGEMENT
-- =============================================

-- Get all roles (for dropdown)
CREATE OR ALTER PROCEDURE sp_GetRoles
AS
BEGIN
    -- Using view for roles; still exclude 'Admin' for dropdown (if business rule required)
    SELECT id, name
    FROM dbo.vw_Roles
    WHERE name <> 'Admin'
    ORDER BY name;
END
GO

-- Get all roles for grid
CREATE OR ALTER PROCEDURE sp_GetRolesForGrid
AS
BEGIN
    SELECT id, name, description, created_at
    FROM dbo.vw_Roles
    ORDER BY name;
END
GO

-- Get all employees
CREATE OR ALTER PROCEDURE sp_GetEmployees
AS
BEGIN
    SELECT *
    FROM dbo.vw_Employees
    ORDER BY name;
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
    -- Use view to ensure consistent join-derived columns if needed later
    SELECT id, name
    FROM dbo.vw_Employees
    ORDER BY name;
END
GO

-- Get shifts by date
CREATE OR ALTER PROCEDURE sp_GetShiftsByDate
    @shift_date DATE
AS
BEGIN
    SELECT *
    FROM dbo.vw_Shifts
    WHERE shift_date = @shift_date
    ORDER BY start_time;
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
    @notes NVARCHAR(500) = NULL, -- Đặt NULL để có thể bỏ qua nếu không cần
    @status NVARCHAR(20)         -- Bắt buộc phải có trạng thái mới
AS
BEGIN
    -- Kiểm tra nếu end_time lớn hơn start_time (tùy chọn)
    -- IF @end_time IS NOT NULL AND @end_time <= @start_time
    -- BEGIN
    --     RAISERROR('Thời gian kết thúc phải lớn hơn thời gian bắt đầu.', 16, 1);
    --     RETURN -1;
    -- END

    INSERT INTO shifts (employee_id, shift_date, start_time, end_time, break_duration, overtime_hours, notes, status)
    VALUES (@employee_id, @shift_date, @start_time, @end_time, @break_duration, @overtime_hours, @notes, @status);
    
    RETURN @@ROWCOUNT; -- Trả về số lượng dòng bị ảnh hưởng
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
    @notes NVARCHAR(500) = NULL, -- Đặt NULL để có thể bỏ qua nếu không cần
    @status NVARCHAR(20)         -- Cập nhật trạng thái
AS
BEGIN
    -- Kiểm tra xem ID có tồn tại không (tùy chọn)
    IF NOT EXISTS (SELECT 1 FROM shifts WHERE id = @id)
    BEGIN
        RAISERROR('Không tìm thấy ID ca làm việc cần cập nhật.', 16, 1);
        RETURN -1;
    END

    UPDATE shifts
    SET employee_id = @employee_id,
        shift_date = @shift_date,
        start_time = @start_time,
        end_time = @end_time,
        break_duration = @break_duration,
        overtime_hours = @overtime_hours,
        notes = @notes,
        status = @status
    WHERE id = @id;

    RETURN @@ROWCOUNT; -- Trả về số lượng dòng bị ảnh hưởng
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
    FROM dbo.vw_Roles
    WHERE name IN ('Admin', 'Manager')
    ORDER BY name;
END
GO

-- Get active employees for user assignment
CREATE OR ALTER PROCEDURE sp_GetActiveEmployees
AS
BEGIN
    SELECT id, name
    FROM dbo.vw_Employees
    WHERE is_active = 1
    ORDER BY id;
END
GO

-- Get all users with details
CREATE OR ALTER PROCEDURE sp_GetUsers
AS
BEGIN
    SELECT id, username, role_id, role_name, employee_id, employee_name, is_active, last_login, created_at, updated_at
    FROM dbo.vw_Users
    ORDER BY id DESC;
END
GO

-- Insert user
CREATE OR ALTER PROCEDURE sp_InsertUser
    @username NVARCHAR(50),
    @password_hash VARBINARY(32),
    @password_salt VARBINARY(16),
    @role_id INT,
    @employee_id INT = NULL,
    @is_active BIT = 1
AS
BEGIN
    INSERT INTO users(username, password_hash, password_salt, role_id, employee_id, is_active, created_at)
    VALUES(@username, @password_hash, @password_salt, @role_id, @employee_id, @is_active, GETDATE());

    RETURN @@ROWCOUNT;
END
GO

-- Update user
CREATE OR ALTER PROCEDURE sp_UpdateUser
    @id INT,
    @role_id INT,
    @employee_id INT = NULL,
    @is_active BIT = 1,
    @password_hash VARBINARY(32) = NULL,
    @password_salt VARBINARY(16) = NULL
AS
BEGIN
    UPDATE users
    SET role_id = @role_id,
        employee_id = @employee_id,
        is_active = @is_active,
        password_hash = CASE WHEN @password_hash IS NOT NULL THEN @password_hash ELSE password_hash END,
        password_salt = CASE WHEN @password_salt IS NOT NULL THEN @password_salt ELSE password_salt END,
        updated_at = GETDATE()
    WHERE id = @id;

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


DROP PROCEDURE IF EXISTS sp_InsertShift;
DROP PROCEDURE IF EXISTS sp_UpdateShift;

USE BookstoreDB;
GO

-- =============================================
-- FIXED STORED PROCEDURES FOR SHIFT MANAGEMENT
-- =============================================

-- Insert shift (Fixed version)
CREATE OR ALTER PROCEDURE sp_InsertShift
    @employee_id INT,
    @shift_date DATE,
    @start_time TIME,
    @end_time TIME,
    @break_duration INT,
    @overtime_hours DECIMAL(4,2),
    @notes NVARCHAR(500) = NULL,
    @status NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Kiểm tra nếu end_time lớn hơn start_time (tùy chọn)
        IF @end_time IS NOT NULL AND @end_time <= @start_time
        BEGIN
            RAISERROR(N'Thời gian kết thúc phải lớn hơn thời gian bắt đầu.', 16, 1);
            RETURN -1;
        END

        -- Kiểm tra employee_id có tồn tại không
        IF NOT EXISTS (SELECT 1 FROM employees WHERE id = @employee_id AND is_active = 1)
        BEGIN
            RAISERROR(N'Nhân viên không tồn tại hoặc không hoạt động.', 16, 1);
            RETURN -1;
        END

        INSERT INTO shifts (employee_id, shift_date, start_time, end_time, break_duration, overtime_hours, notes, status)
        VALUES (@employee_id, @shift_date, @start_time, @end_time, @break_duration, @overtime_hours, @notes, @status);
        
        RETURN @@ROWCOUNT;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
        RETURN -1;
    END CATCH
END
GO

-- Update shift (Fixed version)
CREATE OR ALTER PROCEDURE sp_UpdateShift
    @id INT,
    @employee_id INT,
    @shift_date DATE,
    @start_time TIME,
    @end_time TIME,
    @break_duration INT,
    @overtime_hours DECIMAL(4,2),
    @notes NVARCHAR(500) = NULL,
    @status NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Kiểm tra xem shift ID có tồn tại không
        IF NOT EXISTS (SELECT 1 FROM shifts WHERE id = @id)
        BEGIN
            RAISERROR(N'Không tìm thấy ID ca làm việc cần cập nhật.', 16, 1);
            RETURN -1;
        END

        -- Kiểm tra employee_id có tồn tại không
        IF NOT EXISTS (SELECT 1 FROM employees WHERE id = @employee_id AND is_active = 1)
        BEGIN
            RAISERROR(N'Nhân viên không tồn tại hoặc không hoạt động.', 16, 1);
            RETURN -1;
        END

        -- Kiểm tra thời gian hợp lệ
        IF @end_time IS NOT NULL AND @end_time <= @start_time
        BEGIN
            RAISERROR(N'Thời gian kết thúc phải lớn hơn thời gian bắt đầu.', 16, 1);
            RETURN -1;
        END

        UPDATE shifts
        SET employee_id = @employee_id,
            shift_date = @shift_date,
            start_time = @start_time,
            end_time = @end_time,
            break_duration = @break_duration,
            overtime_hours = @overtime_hours,
            notes = @notes,
            status = @status
        WHERE id = @id;

        RETURN @@ROWCOUNT;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
        RETURN -1;
    END CATCH
END
GO

PRINT N'Fixed Shift Procedures created successfully!';
GO