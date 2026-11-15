using BakeryOrderControl.Models;
using BakeryOrderControl.Repositories;
using BakeryOrderControl.Services;

namespace BakeryOrderControl.UI;

/// <summary>Builds the objects (repositories, services, menus) and runs the main loop.</summary>
public class App
{
    private readonly ProductMenu _productMenu;
    private readonly CustomerMenu _customerMenu;
    private readonly OrderMenu _orderMenu;

    public App()
    {
        InMemoryRepository<Product> productRepo = new();
        InMemoryRepository<Customer> customerRepo = new();
        InMemoryRepository<Order> orderRepo = new();

        ProductService products = new(productRepo, orderRepo);
        CustomerService customers = new(customerRepo, orderRepo);
        OrderService orders = new(orderRepo, customerRepo, productRepo);

        _productMenu = new(products);
        _customerMenu = new(customers);
        _orderMenu = new(orders, customers, products);
    }

    public void Start()
    {
        var exit = false;
        while (!exit)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("===== BAKERY - ORDER CONTROL =====");
                Console.WriteLine("1. Products\n2. Customers\n3. Orders\n0. Exit");

                switch (ConsoleHelper.ReadInt("Option: ", 0, 3))
                {
                    case 1: _productMenu.Show(); break;
                    case 2: _customerMenu.Show(); break;
                    case 3: _orderMenu.Show(); break;
                    case 0: exit = ConsoleHelper.Confirm("Are you sure you want to exit?"); break;
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.ShowError($"Unexpected error: {ex.Message}");
                ConsoleHelper.Pause();
            }
        }
        Console.WriteLine("Goodbye!");
    }
}
