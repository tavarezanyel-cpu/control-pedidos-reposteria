using BakeryOrderControl.Database;
using BakeryOrderControl.Models;
using Microsoft.Data.SqlClient;

namespace BakeryOrderControl.Repositories.SqlServer;

/// <summary>Common logic of the SQL Server repositories: query, search and delete by Id.</summary>
public abstract class SqlServerRepositoryBase<T>(SqlServerDatabase db) : IRepository<T> where T : BaseEntity
{
    protected SqlServerDatabase Db { get; } = db;

    protected abstract string Table { get; }
    protected abstract T Map(SqlDataReader reader);
    protected virtual void Complete(T entity) { }   // loads related data if needed

    public abstract void Add(T entity);
    public abstract void Update(T entity);

    public T? GetById(int id) =>
        Query($"SELECT * FROM {Table} WHERE Id = @id", ("@id", id)).FirstOrDefault();

    public IReadOnlyList<T> GetAll() => Query($"SELECT * FROM {Table} ORDER BY Id");

    public IReadOnlyList<T> Find(Func<T, bool> criteria) => [.. GetAll().Where(criteria)];

    public bool Delete(int id)
    {
        using var connection = Db.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"DELETE FROM {Table} WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id);
        return command.ExecuteNonQuery() > 0;
    }

    protected static int Col(SqlDataReader reader, string name) => reader.GetOrdinal(name);

    private List<T> Query(string sql, params (string Name, object Value)[] parameters)
    {
        List<T> list = [];
        using (var connection = Db.Open())
        using (var command = connection.CreateCommand())
        {
            command.CommandText = sql;
            foreach (var (name, value) in parameters)
                command.Parameters.AddWithValue(name, value);

            using var reader = command.ExecuteReader();
            while (reader.Read()) list.Add(Map(reader));
        }

        foreach (var entity in list) Complete(entity);
        return list;
    }
}
