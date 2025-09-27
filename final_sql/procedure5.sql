
USE BookstoreDB;
GO

-- Thêm ca làm việc mới
CREATE OR ALTER PROCEDURE sp_InsertShift
    @employee_id INT,
    @shift_date DATE,
    @start_time TIME,
    @end_time TIME,
    @break_duration INT = 0,
    @overtime_hours DECIMAL(4,2) = 0,
    @notes NVARCHAR(500) = NULL,
    @status NVARCHAR(20) = N'Mới'
AS
BEGIN    
    -- Validation
    IF NOT EXISTS (SELECT 1 FROM employees WHERE id = @employee_id AND is_active = 1)
        THROW 50001, N'Nhân viên không tồn tại hoặc không hoạt động.', 1;
    
    IF @end_time IS NOT NULL AND @end_time <= @start_time
        THROW 50002, N'Thời gian kết thúc phải lớn hơn thời gian bắt đầu.', 1;
    
    -- Insert operation
    INSERT INTO shifts (employee_id, shift_date, start_time, end_time, 
                       break_duration, overtime_hours, notes, status)
    VALUES (@employee_id, @shift_date, @start_time, @end_time, 
            @break_duration, @overtime_hours, @notes, @status);
    
    RETURN @@ROWCOUNT;
END
GO

-- Cập nhật ca làm việc
CREATE OR ALTER PROCEDURE sp_UpdateShift
    @id INT,
    @employee_id INT,
    @shift_date DATE,
    @start_time TIME,
    @end_time TIME,
    @break_duration INT = 0,
    @overtime_hours DECIMAL(4,2) = 0,
    @notes NVARCHAR(500) = NULL,
    @status NVARCHAR(20) = N'Mới'
AS
BEGIN    
    -- Validation
    IF NOT EXISTS (SELECT 1 FROM shifts WHERE id = @id)
        THROW 50003, N'Ca làm việc không tồn tại.', 1;
        
    IF NOT EXISTS (SELECT 1 FROM employees WHERE id = @employee_id AND is_active = 1)
        THROW 50001, N'Nhân viên không tồn tại hoặc không hoạt động.', 1;
    
    IF @end_time IS NOT NULL AND @end_time <= @start_time
        THROW 50002, N'Thời gian kết thúc phải lớn hơn thời gian bắt đầu.', 1;
    
    -- Update operation
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
END
GO

PRINT N'Tối ưu hóa Shift Procedures hoàn thành!';
GO