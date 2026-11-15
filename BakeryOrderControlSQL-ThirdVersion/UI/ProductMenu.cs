using BakeryOrderControl.Exceptions;
using BakeryOrderControl.Models;
using BakeryOrderControl.Services;

namespace BakeryOrderControl.UI;

public class ProductMenu(ProductService service) : MenuBase
{
    protected override string Title => "PRODUCTS";
    protected override string[] Options => ["Add", "Search", "Edit", "Delete", "List"];

    protected override void Execute(int option)
    {
        Action action = option switch
        {
            1 => Add,
            2 => Search,
            3 => Edit,
            4 => Delete,
            _ => ListAll
        };
        action();
    }

    private void ListAll() =>
        ConsoleHelper.ShowAll(service.GetAll(), "There are no registered products.");

    private void Add()
    {
        var name = ConsoleHelper.ReadText("Name: ");
        var category = ConsoleHelper.ReadText("Category (e.g. Cake, Cookie): ");
        var price = ConsoleHelper.ReadDecimal("Price: ");
        Console.WriteLine($"Product added: {service.Add(name, category, price)}");
    }

    private void Search()
    {
        Console.WriteLine("Search by: 1. ID  2. Name  3. Category  4. Price range");
        switch (ConsoleHelper.ReadInt("Criteria: ", 1, 4))
        {
            case 1:
                List<Product> found = [];
                if (service.FindById(ConsoleHelper.ReadInt("ID: ")) is { } product) found.Add(product);
                ConsoleHelper.ShowAll(found);
                break;
            case 2:
                ConsoleHelper.ShowAll(service.SearchByName(ConsoleHelper.ReadText("Name contains: ")));
                break;
            case 3:
                ConsoleHelper.ShowAll(service.SearchByCategory(ConsoleHelper.ReadText("Category: ")));
                break;
            case 4:
                var min = ConsoleHelper.ReadDecimal("Minimum price: ");
                var max = ConsoleHelper.ReadDecimal("Maximum price: ");
                ConsoleHelper.ShowAll(service.SearchByPriceRange(min, max));
                break;
        }
    }

    private void Edit()
    {
        var id = ConsoleHelper.ReadInt("ID of the product to edit: ");
        var current = service.FindById(id) ?? throw new BakeryException("Product not found.");

        Console.WriteLine($"Current: {current}");
        Console.WriteLine("(Leave empty to keep the current value)");
        var name = ConsoleHelper.ReadText("New name: ", false);
        var category = ConsoleHelper.ReadText("New category: ", false);
        var price = ConsoleHelper.ReadOptionalDecimal("New price: ");

        Console.WriteLine($"Updated: {service.Edit(id, name, category, price)}");
    }

    private void Delete()
    {
        var id = ConsoleHelper.ReadInt("ID of the product to delete: ");
        if (!ConsoleHelper.Confirm("Are you sure you want to delete it?")) { Console.WriteLine("Cancelled."); return; }
        service.Delete(id);
        Console.WriteLine("Product deleted.");
    }
}
