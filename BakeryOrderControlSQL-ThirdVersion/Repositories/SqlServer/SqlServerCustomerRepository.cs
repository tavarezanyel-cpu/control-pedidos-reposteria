using BakeryOrderControl.Database;
using BakeryOrderControl.Models;
using Microsoft.Data.SqlClient;

namespace BakeryOrderControl.Repositories.SqlServer;

public class SqlServerCustomerRepository(SqlServerDatabase db) : SqlServerRepositoryBase<Customer>(db)
{
    protected override string Table => "dbo.Customers";

    protected override Customer Map(SqlDataReader r) =>
        new(r.GetString(Col(r, "Name")),
            r.GetString(Col(r, "Phone")))
        { Id = r.GetInt32(Col(r, "Id")) };

    public override void Add(Customer customer)
    {
        using var connection = Db.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO dbo.Customers (Name, Phone) OUTPUT INSERTED.Id VALUES (@n, @p)";
        command.Parameters.AddWithValue("@n", customer.Name);
        command.Parameters.AddWithValue("@p", customer.Phone);
        customer.Id = Convert.ToInt32(command.ExecuteScalar());
    }

    public override void Update(Customer customer)
    {
        using var connection = Db.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE dbo.Customers SET Name = @n, Phone = @p WHERE Id = @id";
        command.Parameters.AddWithValue("@n", customer.Name);
        command.Parameters.AddWithValue("@p", customer.Phone);
        command.Parameters.AddWithValue("@id", customer.Id);
        command.ExecuteNonQuery();
    }
}
