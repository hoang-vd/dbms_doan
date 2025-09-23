    -- =============================================
    -- Minimal Triggers: Only for employees table
    -- Scope: Roles, Employees, Shifts project
    -- =============================================
    USE BookstoreDB;
    GO

    IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'trg_employees_updated_at')
        DROP TRIGGER trg_employees_updated_at;
    GO

    CREATE TRIGGER trg_employees_updated_at
    ON employees
    AFTER UPDATE
    AS
    BEGIN
        SET NOCOUNT ON;
        UPDATE e
        SET updated_at = GETDATE()
        FROM employees e
        INNER JOIN inserted i ON e.id = i.id;
    END;
    GO

    -- Thêm mô tả cho trigger (sử dụng chuỗi cấp: SCHEMA -> TABLE -> TRIGGER để tránh lỗi 15600)
    -- Nếu property đã tồn tại thì cập nhật, nếu chưa thì thêm mới.
    IF EXISTS (
        SELECT 1 FROM sys.extended_properties ep
        INNER JOIN sys.objects o ON ep.major_id = o.object_id
        WHERE ep.name = 'MS_Description'
          AND o.name = 'trg_employees_updated_at'
    )
        EXEC sys.sp_updateextendedproperty 
            @name=N'MS_Description', 
            @value=N'Trigger tự động cập nhật updated_at khi sửa nhân viên', 
            @level0type=N'SCHEMA', @level0name=N'dbo',
            @level1type=N'TABLE',  @level1name=N'employees',
            @level2type=N'TRIGGER',@level2name=N'trg_employees_updated_at';
    ELSE
        EXEC sys.sp_addextendedproperty 
            @name=N'MS_Description', 
            @value=N'Trigger tự động cập nhật updated_at khi sửa nhân viên', 
            @level0type=N'SCHEMA', @level0name=N'dbo',
            @level1type=N'TABLE',  @level1name=N'employees',
            @level2type=N'TRIGGER',@level2name=N'trg_employees_updated_at';

    PRINT N'Tạo trigger tối giản (employees updated_at) thành công!';
    GO