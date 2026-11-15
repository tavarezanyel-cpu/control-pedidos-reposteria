using BakeryOrderControl.Exceptions;
using BakeryOrderControl.Models;
using BakeryOrderControl.Repositories;

namespace BakeryOrderControl.Services;

public class ProductService(IRepository<Product> products, IRepository<Order> orders)
{
    public Product Add(string name, string category, decimal price)
    {
        var product = new Product(name, category, price);
        products.Add(product);
        return product;
    }

    public IReadOnlyList<Product> GetAll() => products.GetAll();

    public Product? FindById(int id) => products.GetById(id);

    public IReadOnlyList<Product> SearchByName(string text) =>
        products.Find(p => p.Name.Contains(text, StringComparison.OrdinalIgnoreCase));

    public IReadOnlyList<Product> SearchByCategory(string text) =>
        products.Find(p => p.Category.Contains(text, StringComparison.OrdinalIgnoreCase));

    public IReadOnlyList<Product> SearchByPriceRange(decimal min, decimal max) =>
        products.Find(p => p.Price >= min && p.Price <= max);

    public Product Edit(int id, string? name, string? category, decimal? price)
    {
        var product = products.GetById(id) ?? throw new BakeryException("Product not found.");

        if (!string.IsNullOrWhiteSpace(name)) product.Name = name;
        if (!string.IsNullOrWhiteSpace(category)) product.Category = category;
        if (price is { } newPrice) product.Price = newPrice;

        products.Update(product);
        return product;
    }

    public void Delete(int id)
    {
        if (products.GetById(id) is null)
            throw new BakeryException("Product not found.");
        if (orders.Find(o => o.Items.ContainsKey(id)).Count > 0)
            throw new BakeryException("Cannot delete: the product is in one or more orders.");
        products.Delete(id);
    }
}
