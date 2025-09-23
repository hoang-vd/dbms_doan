USE master;
GO

IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'BookstoreDB')
BEGIN
    CREATE DATABASE BookstoreDB;
END
GO

USE BookstoreDB;
GO


-- 1. ROLES TABLE
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='roles' AND xtype='U')
BEGIN
    CREATE TABLE roles (
        id INT IDENTITY(1,1) PRIMARY KEY,
        name NVARCHAR(50) NOT NULL UNIQUE,
        description NVARCHAR(255),
        created_at DATETIME DEFAULT GETDATE()
    );
END
GO

-- Insert default roles
IF NOT EXISTS (SELECT * FROM roles WHERE name = 'Admin')
BEGIN
    INSERT INTO roles (name, description) VALUES
    ('Admin', N'Quản trị viên hệ thống'),
    ('Manager', N'Quản lý cửa hàng'),
    ('Cashier', N'Nhân viên thu ngân'),
    ('Warehouse', N'Nhân viên kho');
END
GO

-- 2. EMPLOYEES TABLE (References roles)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='employees' AND xtype='U')
BEGIN
    CREATE TABLE employees (
        id INT IDENTITY(1,1) PRIMARY KEY,
        name NVARCHAR(100) NOT NULL,
        phone NVARCHAR(20),
        email NVARCHAR(100) UNIQUE,
        address NVARCHAR(255) NULL,
        birth_date DATE NULL,
        role_id INT NOT NULL,
        hire_date DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
        salary DECIMAL(12,2) CHECK (salary >= 0),
        is_active BIT DEFAULT 1,
        created_at DATETIME DEFAULT GETDATE(),
        updated_at DATETIME DEFAULT GETDATE(),
        
        CONSTRAINT FK_employees_role_id FOREIGN KEY (role_id) REFERENCES roles(id),
        CONSTRAINT CHK_employees_email CHECK (email LIKE '%@%.%')
    );
END
GO


-- 3. SHIFTS TABLE (References employees)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='shifts' AND xtype='U')
BEGIN
    CREATE TABLE shifts (
        id INT IDENTITY(1,1) PRIMARY KEY,
        employee_id INT NOT NULL,
        shift_date DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
        start_time TIME NOT NULL,
        end_time TIME,
        break_duration INT DEFAULT 0, -- Minutes
        total_hours AS (
            CASE 
                WHEN end_time IS NOT NULL 
                THEN DATEDIFF(MINUTE, start_time, end_time) - ISNULL(break_duration, 0)
                ELSE NULL 
            END / 60.0
        ) PERSISTED,
        overtime_hours DECIMAL(4,2) DEFAULT 0,
        status NVARCHAR(20) DEFAULT 'Scheduled' CHECK (status IN ('Scheduled', 'Started', 'Completed', 'Cancelled')),
        notes NVARCHAR(500),
        created_at DATETIME DEFAULT GETDATE(),
        
        CONSTRAINT FK_shifts_employee_id FOREIGN KEY (employee_id) REFERENCES employees(id),
        CONSTRAINT CHK_shifts_time CHECK (end_time IS NULL OR end_time > start_time)
    );
END
GO
-- =============================================
-- INDEXES (Only remaining tables)
-- =============================================
CREATE NONCLUSTERED INDEX IX_roles_name ON roles(name);
CREATE NONCLUSTERED INDEX IX_employees_email ON employees(email);
CREATE NONCLUSTERED INDEX IX_employees_role_id ON employees(role_id);
CREATE NONCLUSTERED INDEX IX_employees_is_active ON employees(is_active);
-- Shifts indexes
CREATE NONCLUSTERED INDEX IX_shifts_employee_id ON shifts(employee_id);
CREATE NONCLUSTERED INDEX IX_shifts_shift_date ON shifts(shift_date);
CREATE NONCLUSTERED INDEX IX_shifts_status ON shifts(status);
-- Extended properties (optional)
EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description', @value=N'Bảng vai trò nhân viên',
    @level0type=N'SCHEMA', @level0name=N'dbo',
    @level1type=N'TABLE', @level1name=N'roles';

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description', @value=N'Bảng thông tin nhân viên',
    @level0type=N'SCHEMA', @level0name=N'dbo',
    @level1type=N'TABLE', @level1name=N'employees';

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description', @value=N'Bảng ca làm việc',
    @level0type=N'SCHEMA', @level0name=N'dbo',
    @level1type=N'TABLE', @level1name=N'shifts';

PRINT N'Tạo bảng (roles, employees, shifts) thành công!';
GO