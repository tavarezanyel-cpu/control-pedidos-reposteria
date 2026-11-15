using BakeryOrderControl.Database;
using BakeryOrderControl.Models;
using Microsoft.Data.SqlClient;

namespace BakeryOrderControl.Repositories.SqlServer;

public class SqlServerProductRepository(SqlServerDatabase db) : SqlServerRepositoryBase<Product>(db)
{
    protected override string Table => "dbo.Products";

    protected override Product Map(SqlDataReader r) =>
        new(r.GetString(Col(r, "Name")),
            r.GetString(Col(r, "Category")),
            r.GetDecimal(Col(r, "Price")))
        { Id = r.GetInt32(Col(r, "Id")) };

    public override void Add(Product product)
    {
        using var connection = Db.Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            "INSERT INTO dbo.Products (Name, Category, Price) OUTPUT INSERTED.Id VALUES (@n, @c, @p)";
        command.Parameters.AddWithValue("@n", product.Name);
        command.Parameters.AddWithValue("@c", product.Category);
        command.Parameters.AddWithValue("@p", product.Price);
        product.Id = Convert.ToInt32(command.ExecuteScalar());
    }

    public override void Update(Product product)
    {
        using var connection = Db.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE dbo.Products SET Name = @n, Category = @c, Price = @p WHERE Id = @id";
        command.Parameters.AddWithValue("@n", product.Name);
        command.Parameters.AddWithValue("@c", product.Category);
        command.Parameters.AddWithValue("@p", product.Price);
        command.Parameters.AddWithValue("@id", product.Id);
        command.ExecuteNonQuery();
    }
}
