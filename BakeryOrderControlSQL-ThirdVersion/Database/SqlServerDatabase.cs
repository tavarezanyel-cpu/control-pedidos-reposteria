using Microsoft.Data.SqlClient;

namespace BakeryOrderControl.Database;

/// <summary>Handles the SQL Server connection and creates the database and tables if they do not exist.</summary>
public class SqlServerDatabase(string connectionString)
{
    private string DatabaseName => new SqlConnectionStringBuilder(connectionString).InitialCatalog;

    public SqlConnection Open()
    {
        var connection = new SqlConnection(connectionString);
        connection.Open();
        return connection;
    }

    public void InitializeSchema()
    {
        CreateDatabaseIfMissing();

        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = Schema;
        command.ExecuteNonQuery();
    }

    private void CreateDatabaseIfMissing()
    {
        // Connect to the master database first, because our database may not exist yet
        var master = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = "master" };
        using var connection = new SqlConnection(master.ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "IF DB_ID(@name) IS NULL EXEC('CREATE DATABASE [' + @name + ']')";
        command.Parameters.AddWithValue("@name", DatabaseName);
        command.ExecuteNonQuery();
    }

    // Same tables as docs/database_model.sql
    private const string Schema = """
        IF OBJECT_ID(N'dbo.Products', N'U') IS NULL
        CREATE TABLE dbo.Products (
            Id       INT IDENTITY(1,1) PRIMARY KEY,
            Name     NVARCHAR(200) NOT NULL,
            Category NVARCHAR(200) NOT NULL,
            Price    DECIMAL(10,2) NOT NULL CONSTRAINT CK_Products_Price CHECK (Price > 0)
        );
        
        IF OBJECT_ID(N'dbo.Customers', N'U') IS NULL
        CREATE TABLE dbo.Customers (
            Id    INT IDENTITY(1,1) PRIMARY KEY,
            Name  NVARCHAR(200) NOT NULL,
            Phone NVARCHAR(50)  NOT NULL
        );
        
        IF OBJECT_ID(N'dbo.Orders', N'U') IS NULL
        CREATE TABLE dbo.Orders (
            Id         INT IDENTITY(1,1) PRIMARY KEY,
            CustomerId INT NOT NULL CONSTRAINT FK_Orders_Customers REFERENCES dbo.Customers (Id),
            OrderDate  DATETIME2(0) NOT NULL,
            Delivered  BIT NOT NULL CONSTRAINT DF_Orders_Delivered DEFAULT 0
        );
        
        IF OBJECT_ID(N'dbo.OrderItems', N'U') IS NULL
        CREATE TABLE dbo.OrderItems (
            OrderId   INT NOT NULL CONSTRAINT FK_OrderItems_Orders   REFERENCES dbo.Orders (Id) ON DELETE CASCADE,
            ProductId INT NOT NULL CONSTRAINT FK_OrderItems_Products REFERENCES dbo.Products (Id),
            Quantity  INT NOT NULL CONSTRAINT CK_OrderItems_Quantity CHECK (Quantity > 0),
            CONSTRAINT PK_OrderItems PRIMARY KEY (OrderId, ProductId)
        );
        """;
}
