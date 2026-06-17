CREATE OR ALTER PROCEDURE sp_GetCustomers
AS
BEGIN
    SELECT *
    FROM Customers;
END
GO

CREATE OR ALTER PROCEDURE sp_GetDashboard
AS
BEGIN
    SELECT * FROM Customers;
    SELECT * FROM Orders;
END
GO

CREATE OR ALTER PROCEDURE sp_CreateCustomer
(
    @Name NVARCHAR(100),
    @CustomerId INT OUTPUT
)
AS
BEGIN
    INSERT INTO Customers(Name)
    VALUES(@Name);

    SET @CustomerId = SCOPE_IDENTITY();
END
GO