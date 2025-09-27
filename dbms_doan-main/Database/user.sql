CREATE TABLE users (
    id INT IDENTITY(1,1) PRIMARY KEY,
    username NVARCHAR(50) NOT NULL UNIQUE,
    password_hash VARBINARY(MAX) NOT NULL,
    password_salt VARBINARY(MAX) NOT NULL,
    employee_id INT NULL, -- tuỳ chọn liên kết tới bảng employees
    role_id INT NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_users_role_id FOREIGN KEY (role_id) REFERENCES roles(id)
);
