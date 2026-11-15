using BakeryOrderControl.Models;

namespace BakeryOrderControl.Repositories;

/// <summary>Generic repository that keeps the entities in an in-memory list.</summary>
public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly List<T> _data = [];
    private int _nextId = 1;

    public void Add(T entity)
    {
        entity.Id = _nextId++;
        _data.Add(entity);
    }

    // In memory the object is already the same reference, so there is nothing to save.
    public void Update(T entity) { }

    public T? GetById(int id) => _data.FirstOrDefault(e => e.Id == id);

    public IReadOnlyList<T> GetAll() => [.. _data];

    public IReadOnlyList<T> Find(Func<T, bool> criteria) => [.. _data.Where(criteria)];

    public bool Delete(int id) => GetById(id) is { } entity && _data.Remove(entity);
}
