using BakeryOrderControl.Exceptions;

namespace BakeryOrderControl.Models;

public class Product : BaseEntity
{
    private string _name = "";
    private string _category = "";
    private decimal _price;

    public string Name
    {
        get => _name;
        set => _name = string.IsNullOrWhiteSpace(value)
            ? throw new BakeryException("The product name cannot be empty.")
            : value.Trim();
    }

    public string Category
    {
        get => _category;
        set => _category = string.IsNullOrWhiteSpace(value)
            ? throw new BakeryException("The category cannot be empty.")
            : value.Trim();
    }

    public decimal Price
    {
        get => _price;
        set => _price = value > 0
            ? value
            : throw new BakeryException("The price must be greater than 0.");
    }

    public Product(string name, string category, decimal price)
    {
        Name = name;
        Category = category;
        Price = price;
    }

    public override string Describe() => $"[{Id}] {Name} | {Category} | RD$ {Price:N2}";
}
