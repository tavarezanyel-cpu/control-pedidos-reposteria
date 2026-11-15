using BakeryOrderControl.Models;

namespace BakeryOrderControl.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    void Add(T entity);
    void Update(T entity);
    T? GetById(int id);
    IReadOnlyList<T> GetAll();
    IReadOnlyList<T> Find(Func<T, bool> criteria);
    bool Delete(int id);
}
