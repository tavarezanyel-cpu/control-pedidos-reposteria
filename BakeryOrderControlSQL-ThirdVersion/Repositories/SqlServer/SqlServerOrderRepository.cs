using BakeryOrderControl.Database;
using BakeryOrderControl.Models;
using Microsoft.Data.SqlClient;

namespace BakeryOrderControl.Repositories.SqlServer;

/// <summary>Saves the order in the Orders table and its products in OrderItems.</summary>
public class SqlServerOrderRepository(SqlServerDatabase db) : SqlServerRepositoryBase<Order>(db)
{
    protected override string Table => "dbo.Orders";

    protected override Order Map(SqlDataReader r) =>
        new(r.GetInt32(Col(r, "Id")),
            r.GetInt32(Col(r, "CustomerId")),
            r.GetDateTime(Col(r, "OrderDate")),
            r.GetBoolean(Col(r, "Delivered")));

    // Loads the products (OrderItems) of an order that was already read
    protected override void Complete(Order order)
    {
        using var connection = Db.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT ProductId, Quantity FROM dbo.OrderItems WHERE OrderId = @id";
        command.Parameters.AddWithValue("@id", order.Id);
        using var reader = command.ExecuteReader();
        while (reader.Read())
            order.LoadItem(reader.GetInt32(0), reader.GetInt32(1));
    }

    public override void Add(Order order)
    {
        using var connection = Db.Open();
        using var transaction = connection.BeginTransaction();

        using (var command = connection.CreateCommand())
        {
            command.Transaction = transaction;
            command.CommandText =
                "INSERT INTO dbo.Orders (CustomerId, OrderDate, Delivered) OUTPUT INSERTED.Id VALUES (@c, @d, @e)";
            command.Parameters.AddWithValue("@c", order.CustomerId);
            command.Parameters.AddWithValue("@d", order.Date);
            command.Parameters.AddWithValue("@e", order.Delivered);
            order.Id = Convert.ToInt32(command.ExecuteScalar());
        }

        SaveItems(connection, transaction, order);
        transaction.Commit();
    }

    public override void Update(Order order)
    {
        using var connection = Db.Open();
        using var transaction = connection.BeginTransaction();

        using (var command = connection.CreateCommand())
        {
            command.Transaction = transaction;
            command.CommandText = "UPDATE dbo.Orders SET Delivered = @e WHERE Id = @id";
            command.Parameters.AddWithValue("@e", order.Delivered);
            command.Parameters.AddWithValue("@id", order.Id);
            command.ExecuteNonQuery();
        }

        SaveItems(connection, transaction, order);
        transaction.Commit();
    }

    private static void SaveItems(SqlConnection connection, SqlTransaction transaction, Order order)
    {
        using (var delete = connection.CreateCommand())
        {
            delete.Transaction = transaction;
            delete.CommandText = "DELETE FROM dbo.OrderItems WHERE OrderId = @id";
            delete.Parameters.AddWithValue("@id", order.Id);
            delete.ExecuteNonQuery();
        }

        foreach (var (productId, quantity) in order.Items)
        {
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = "INSERT INTO dbo.OrderItems (OrderId, ProductId, Quantity) VALUES (@o, @p, @q)";
            command.Parameters.AddWithValue("@o", order.Id);
            command.Parameters.AddWithValue("@p", productId);
            command.Parameters.AddWithValue("@q", quantity);
            command.ExecuteNonQuery();
        }
    }
}
