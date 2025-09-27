/* Migration script to align existing shifts.status values & constraint with new allowed values ('Mới','Lâu năm') */
USE BookstoreDB;
GO

/* 1. If old English statuses existed, map them to new Vietnamese set */
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.shifts') AND name='status')
BEGIN
    UPDATE shifts SET status = N'Mới'     WHERE status IN ('Scheduled','New');
    UPDATE shifts SET status = N'Lâu năm' WHERE status IN ('Started','Completed','Cancelled');
END
GO

/* 2. Drop existing CHECK constraint on status (name may vary). Find name dynamically */
DECLARE @constraintName sysname;
SELECT @constraintName = cc.name
FROM sys.check_constraints cc
JOIN sys.columns c ON c.object_id = cc.parent_object_id AND c.column_id = cc.parent_column_id
WHERE cc.parent_object_id = OBJECT_ID('dbo.shifts')
  AND c.name = 'status';
IF @constraintName IS NOT NULL
BEGIN
    PRINT 'Dropping constraint ' + @constraintName;
    EXEC('ALTER TABLE dbo.shifts DROP CONSTRAINT ' + QUOTENAME(@constraintName));
END
GO

/* 3. Create new constraint for Vietnamese values */
ALTER TABLE dbo.shifts
ADD CONSTRAINT CHK_shifts_status CHECK (status IN (N'Mới', N'Lâu năm'));
GO

PRINT N'Migration for shift status completed.';
