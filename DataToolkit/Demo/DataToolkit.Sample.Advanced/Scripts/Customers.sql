CREATE TABLE Customers
(
    Id INT IDENTITY(1,1) NOT NULL,

    Name NVARCHAR(100) NOT NULL,

    Email NVARCHAR(200) NULL,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Customers_CreatedAt
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_Customers
        PRIMARY KEY (Id)
);
GO


INSERT INTO Customers (Name, Email)
VALUES
('Juan Perez', 'juan@test.com'),
('Maria Gomez', 'maria@test.com'),
('Carlos Ruiz', 'carlos@test.com');
GO
