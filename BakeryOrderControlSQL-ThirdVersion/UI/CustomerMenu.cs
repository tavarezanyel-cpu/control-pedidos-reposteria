using BakeryOrderControl.Exceptions;
using BakeryOrderControl.Models;
using BakeryOrderControl.Services;

namespace BakeryOrderControl.UI;

public class CustomerMenu(CustomerService service) : MenuBase
{
    protected override string Title => "CUSTOMERS";
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
        ConsoleHelper.ShowAll(service.GetAll(), "There are no registered customers.");

    private void Add()
    {
        var name = ConsoleHelper.ReadText("Name: ");
        var phone = ConsoleHelper.ReadText("Phone: ");
        Console.WriteLine($"Customer added: {service.Add(name, phone)}");
    }

    private void Search()
    {
        Console.WriteLine("Search by: 1. ID  2. Name  3. Phone");
        switch (ConsoleHelper.ReadInt("Criteria: ", 1, 3))
        {
            case 1:
                List<Customer> found = [];
                if (service.FindById(ConsoleHelper.ReadInt("ID: ")) is { } customer) found.Add(customer);
                ConsoleHelper.ShowAll(found);
                break;
            case 2:
                ConsoleHelper.ShowAll(service.SearchByName(ConsoleHelper.ReadText("Name contains: ")));
                break;
            case 3:
                ConsoleHelper.ShowAll(service.SearchByPhone(ConsoleHelper.ReadText("Phone contains: ")));
                break;
        }
    }

    private void Edit()
    {
        var id = ConsoleHelper.ReadInt("ID of the customer to edit: ");
        var current = service.FindById(id) ?? throw new BakeryException("Customer not found.");

        Console.WriteLine($"Current: {current}");
        Console.WriteLine("(Leave empty to keep the current value)");
        var name = ConsoleHelper.ReadText("New name: ", false);
        var phone = ConsoleHelper.ReadText("New phone: ", false);

        Console.WriteLine($"Updated: {service.Edit(id, name, phone)}");
    }

    private void Delete()
    {
        var id = ConsoleHelper.ReadInt("ID of the customer to delete: ");
        if (!ConsoleHelper.Confirm("Are you sure you want to delete it?")) { Console.WriteLine("Cancelled."); return; }
        service.Delete(id);
        Console.WriteLine("Customer deleted.");
    }
}
