using BakeryOrderControl.Exceptions;
using BakeryOrderControl.Models;
using BakeryOrderControl.Services;

namespace BakeryOrderControl.UI;

public class OrderMenu(OrderService orders, CustomerService customers, ProductService products) : MenuBase
{
    protected override string Title => "ORDERS";
    protected override string[] Options =>
        ["Create order", "Search", "Edit quantities", "Mark as delivered", "Delete", "List"];

    protected override void Execute(int option)
    {
        Action action = option switch
        {
            1 => Create,
            2 => Search,
            3 => EditQuantities,
            4 => Deliver,
            5 => Delete,
            _ => ListAll
        };
        action();
    }

    private void ShowOrders(IEnumerable<Order> list)
    {
        List<Order> found = [.. list];
        if (found.Count == 0) { Console.WriteLine("No results."); return; }
        foreach (var order in found) Console.WriteLine(orders.DescribeDetails(order) + "\n");
    }

    private void ListAll()
    {
        if (orders.GetAll().Count == 0) { Console.WriteLine("There are no registered orders."); return; }
        ShowOrders(orders.GetAll());
    }

    private void Create()
    {
        if (customers.GetAll().Count == 0 || products.GetAll().Count == 0)
            throw new BakeryException("You must register at least one customer and one product first.");

        ConsoleHelper.ShowAll(customers.GetAll());
        var customerId = ConsoleHelper.ReadInt("Customer ID: ");
        if (customers.FindById(customerId) is null)
            throw new BakeryException("Customer not found.");

        Dictionary<int, int> items = new();   // ProductId -> Quantity
        var addMore = true;
        while (addMore)
        {
            ConsoleHelper.ShowAll(products.GetAll());
            var productId = ConsoleHelper.ReadInt("Product ID: ");
            if (products.FindById(productId) is null)
            {
                ConsoleHelper.ShowError("Product not found.");
            }
            else
            {
                var quantity = ConsoleHelper.ReadInt("Quantity: ", 1, 1000);
                items[productId] = items.GetValueOrDefault(productId) + quantity;
                Console.WriteLine("Product added to the order.");
            }
            addMore = ConsoleHelper.Confirm("Add another product?");
        }

        var order = orders.Create(customerId, items);
        Console.WriteLine("Order created:");
        Console.WriteLine(orders.DescribeDetails(order));
    }

    private void Search()
    {
        Console.WriteLine("Search by: 1. ID  2. Customer name  3. Status");
        switch (ConsoleHelper.ReadInt("Criteria: ", 1, 3))
        {
            case 1:
                List<Order> found = [];
                if (orders.FindById(ConsoleHelper.ReadInt("ID: ")) is { } order) found.Add(order);
                ShowOrders(found);
                break;
            case 2:
                ShowOrders(orders.SearchByCustomerName(ConsoleHelper.ReadText("Customer name contains: ")));
                break;
            case 3:
                Console.WriteLine("1. Pending  2. Delivered");
                ShowOrders(orders.SearchByStatus(ConsoleHelper.ReadInt("Status: ", 1, 2) == 2));
                break;
        }
    }

    private void EditQuantities()
    {
        var orderId = ConsoleHelper.ReadInt("Order ID: ");
        var order = orders.FindById(orderId) ?? throw new BakeryException("Order not found.");
        Console.WriteLine(orders.DescribeDetails(order));

        var productId = ConsoleHelper.ReadInt("ID of the product to edit/add: ");
        var quantity = ConsoleHelper.ReadInt("New quantity (0 to remove it): ", 0, 1000);

        Console.WriteLine("Order updated:");
        Console.WriteLine(orders.DescribeDetails(orders.ChangeQuantity(orderId, productId, quantity)));
    }

    private void Deliver()
    {
        orders.MarkAsDelivered(ConsoleHelper.ReadInt("Order ID: "));
        Console.WriteLine("Order marked as delivered.");
    }

    private void Delete()
    {
        var id = ConsoleHelper.ReadInt("ID of the order to delete: ");
        if (!ConsoleHelper.Confirm("Are you sure you want to delete it?")) { Console.WriteLine("Cancelled."); return; }
        orders.Delete(id);
        Console.WriteLine("Order deleted.");
    }
}
