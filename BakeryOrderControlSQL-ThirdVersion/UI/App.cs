using BakeryOrderControl.Database;
using BakeryOrderControl.Models;
using BakeryOrderControl.Repositories;
using BakeryOrderControl.Repositories.SqlServer;
using BakeryOrderControl.Services;

namespace BakeryOrderControl.UI;

/// <summary>Builds the objects (database, repositories, services, menus) and runs the main loop.</summary>
public class App
{
    // SQL Server connection. If your server has another name, change "Server=" here:
    //   LocalDB (comes with Visual Studio): (localdb)\MSSQLLocalDB
    //   SQL Server Express:                 .\SQLEXPRESS
    //   Default SQL Server instance:        localhost
    private const string ConnectionString =
        """Server=(localdb)\MSSQLLocalDB;Database=BakeryOrderControl;Trusted_Connection=True;TrustServerCertificate=True;""";

    private readonly ProductMenu _productMenu;
    private readonly CustomerMenu _customerMenu;
    private readonly OrderMenu _orderMenu;

    public App()
    {
        // The database and its tables are created automatically the first time
        SqlServerDatabase database = new(ConnectionString);
        database.InitializeSchema();

        // To go back to in-memory data, use InMemoryRepository<T> here instead
        IRepository<Product> productRepo = new SqlServerProductRepository(database);
        IRepository<Customer> customerRepo = new SqlServerCustomerRepository(database);
        IRepository<Order> orderRepo = new SqlServerOrderRepository(database);

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
