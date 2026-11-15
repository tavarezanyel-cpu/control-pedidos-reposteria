using System.Globalization;

// ===================== IN-MEMORY DATA =====================
List<Product> products = [];
List<Customer> customers = [];
List<Order> orders = [];
int nextProductId = 1, nextCustomerId = 1, nextOrderId = 1;

// ===================== MAIN LOOP =====================
var exit = false;
while (!exit)
{
    try
    {
        Console.Clear();
        Console.WriteLine("===== BAKERY - ORDER CONTROL =====");
        Console.WriteLine("1. Products\n2. Customers\n3. Orders\n0. Exit");

        switch (ReadInt("Option: ", 0, 3))
        {
            case 1: ProductsMenu(); break;
            case 2: CustomersMenu(); break;
            case 3: OrdersMenu(); break;
            case 0: exit = Confirm("Are you sure you want to exit?"); break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unexpected error: {ex.Message}");
        Pause();
    }
}
Console.WriteLine("Goodbye!");

// ===================== SAFE INPUT =====================
string ReadText(string message, bool required = true)
{
    while (true)
    {
        Console.Write(message);
        var text = (Console.ReadLine() ?? "").Trim();
        if (!required || text.Length > 0) return text;
        Console.WriteLine("  ! This field cannot be empty.");
    }
}

int ReadInt(string message, int min = 1, int max = int.MaxValue)
{
    while (true)
    {
        Console.Write(message);
        if (int.TryParse(Console.ReadLine(), out var number) && number >= min && number <= max) return number;
        Console.WriteLine($"  ! Enter a whole number between {min} and {(max == int.MaxValue ? "infinity" : max.ToString())}.");
    }
}

bool TryParseDecimal(string? text, out decimal value) =>
    decimal.TryParse((text ?? "").Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out value);

decimal ReadDecimal(string message)
{
    while (true)
    {
        Console.Write(message);
        if (TryParseDecimal(Console.ReadLine(), out var value) && value > 0) return value;
        Console.WriteLine("  ! Enter a valid amount greater than 0.");
    }
}

bool Confirm(string message)
{
    Console.Write($"{message} (Y/N): ");
    return (Console.ReadLine() ?? "").Trim().Equals("Y", StringComparison.OrdinalIgnoreCase);
}

void Pause()
{
    Console.Write("\nPress Enter to continue...");
    Console.ReadLine();
}

void RunMenu(string title, string[] options, Action<int> execute)
{
    var back = false;
    while (!back)
    {
        Console.Clear();
        Console.WriteLine($"--- {title} ---");
        for (var i = 0; i < options.Length; i++)
            Console.WriteLine($"{i + 1}. {options[i]}");
        Console.WriteLine("0. Back");

        var option = ReadInt("Option: ", 0, options.Length);
        if (option == 0) { back = true; continue; }

        try
        {
            execute(option);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ! Error: {ex.Message}");
        }
        Pause();
    }
}

// ===================== PRODUCTS =====================
void ProductsMenu() => RunMenu("PRODUCTS", ["Add", "Search", "Edit", "Delete", "List"], option =>
{
    Action action = option switch
    {
        1 => AddProduct,
        2 => SearchProduct,
        3 => EditProduct,
        4 => DeleteProduct,
        _ => ListProducts
    };
    action();
});

void AddProduct()
{
    var product = new Product(ReadText("Name: "), ReadText("Category (e.g. Cake, Cookie): "), ReadDecimal("Price: "))
    {
        Id = nextProductId++
    };
    products.Add(product);
    Console.WriteLine($"Product added: {product}");
}

void ListProducts()
{
    if (products.Count == 0) { Console.WriteLine("There are no registered products."); return; }
    foreach (var product in products) Console.WriteLine(product);
}

void SearchProduct()
{
    Console.WriteLine("Search by: 1. ID  2. Name  3. Category  4. Price range");
    List<Product> results = [];

    switch (ReadInt("Criteria: ", 1, 4))
    {
        case 1:
            var id = ReadInt("ID: ");
            results = [.. products.Where(p => p.Id == id)];
            break;
        case 2:
            var name = ReadText("Name contains: ");
            results = [.. products.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase))];
            break;
        case 3:
            var category = ReadText("Category: ");
            results = [.. products.Where(p => p.Category.Contains(category, StringComparison.OrdinalIgnoreCase))];
            break;
        case 4:
            var min = ReadDecimal("Minimum price: ");
            var max = ReadDecimal("Maximum price: ");
            results = [.. products.Where(p => p.Price >= min && p.Price <= max)];
            break;
    }

    if (results.Count == 0) Console.WriteLine("No results.");
    foreach (var product in results) Console.WriteLine(product);
}

void EditProduct()
{
    var id = ReadInt("ID of the product to edit: ");
    if (products.FirstOrDefault(p => p.Id == id) is not { } product)
    {
        Console.WriteLine("Product not found.");
        return;
    }

    Console.WriteLine($"Current: {product}");
    Console.WriteLine("(Leave empty to keep the current value)");
    var name = ReadText("New name: ", false);
    var category = ReadText("New category: ", false);
    var price = ReadText("New price: ", false);

    if (name != "") product.Name = name;
    if (category != "") product.Category = category;
    if (price != "")
    {
        if (TryParseDecimal(price, out var value) && value > 0) product.Price = value;
        else Console.WriteLine("Invalid price, the previous one was kept.");
    }
    Console.WriteLine($"Updated: {product}");
}

void DeleteProduct()
{
    var id = ReadInt("ID of the product to delete: ");
    if (products.FirstOrDefault(p => p.Id == id) is not { } product)
    {
        Console.WriteLine("Product not found.");
        return;
    }
    if (orders.Any(o => o.Items.ContainsKey(id)))
    {
        Console.WriteLine("Cannot delete: the product is in one or more orders.");
        return;
    }
    products.Remove(product);
    Console.WriteLine("Product deleted.");
}

// ===================== CUSTOMERS =====================
void CustomersMenu() => RunMenu("CUSTOMERS", ["Add", "Search", "Edit", "Delete", "List"], option =>
{
    Action action = option switch
    {
        1 => AddCustomer,
        2 => SearchCustomer,
        3 => EditCustomer,
        4 => DeleteCustomer,
        _ => ListCustomers
    };
    action();
});

void AddCustomer()
{
    var customer = new Customer(ReadText("Name: "), ReadText("Phone: "))
    {
        Id = nextCustomerId++
    };
    customers.Add(customer);
    Console.WriteLine($"Customer added: {customer}");
}

void ListCustomers()
{
    if (customers.Count == 0) { Console.WriteLine("There are no registered customers."); return; }
    foreach (var customer in customers) Console.WriteLine(customer);
}

void SearchCustomer()
{
    Console.WriteLine("Search by: 1. ID  2. Name  3. Phone");
    List<Customer> results = [];

    switch (ReadInt("Criteria: ", 1, 3))
    {
        case 1:
            var id = ReadInt("ID: ");
            results = [.. customers.Where(c => c.Id == id)];
            break;
        case 2:
            var name = ReadText("Name contains: ");
            results = [.. customers.Where(c => c.Name.Contains(name, StringComparison.OrdinalIgnoreCase))];
            break;
        case 3:
            var phone = ReadText("Phone contains: ");
            results = [.. customers.Where(c => c.Phone.Contains(phone))];
            break;
    }

    if (results.Count == 0) Console.WriteLine("No results.");
    foreach (var customer in results) Console.WriteLine(customer);
}

void EditCustomer()
{
    var id = ReadInt("ID of the customer to edit: ");
    if (customers.FirstOrDefault(c => c.Id == id) is not { } customer)
    {
        Console.WriteLine("Customer not found.");
        return;
    }

    Console.WriteLine($"Current: {customer}");
    Console.WriteLine("(Leave empty to keep the current value)");
    var name = ReadText("New name: ", false);
    var phone = ReadText("New phone: ", false);
    if (name != "") customer.Name = name;
    if (phone != "") customer.Phone = phone;
    Console.WriteLine($"Updated: {customer}");
}

void DeleteCustomer()
{
    var id = ReadInt("ID of the customer to delete: ");
    if (customers.FirstOrDefault(c => c.Id == id) is not { } customer)
    {
        Console.WriteLine("Customer not found.");
        return;
    }
    if (orders.Any(o => o.CustomerId == id))
    {
        Console.WriteLine("Cannot delete: the customer has registered orders.");
        return;
    }
    customers.Remove(customer);
    Console.WriteLine("Customer deleted.");
}

// ===================== ORDERS =====================
void OrdersMenu() => RunMenu("ORDERS",
    ["Create order", "Search", "Edit quantities", "Mark as delivered", "Delete", "List"], option =>
{
    Action action = option switch
    {
        1 => CreateOrder,
        2 => SearchOrder,
        3 => EditOrder,
        4 => MarkAsDelivered,
        5 => DeleteOrder,
        _ => ListOrders
    };
    action();
});

string CustomerName(int id) => customers.FirstOrDefault(c => c.Id == id)?.Name ?? "(unknown)";

decimal OrderTotal(Order order) =>
    order.Items.Sum(i => (products.FirstOrDefault(p => p.Id == i.Key)?.Price ?? 0) * i.Value);

void ShowOrder(Order order)
{
    Console.WriteLine($"Order #{order.Id} | Customer: {CustomerName(order.CustomerId)} | {order.Date:dd/MM/yyyy HH:mm} | {(order.Delivered ? "DELIVERED" : "PENDING")}");
    foreach (var (productId, quantity) in order.Items)
    {
        var productName = products.FirstOrDefault(p => p.Id == productId)?.Name ?? "(deleted)";
        Console.WriteLine($"    - {productName} x{quantity}");
    }
    Console.WriteLine($"    Total: RD$ {OrderTotal(order):N2}");
}

void CreateOrder()
{
    if (customers.Count == 0 || products.Count == 0)
    {
        Console.WriteLine("You must register at least one customer and one product first.");
        return;
    }

    ListCustomers();
    var customerId = ReadInt("Customer ID: ");
    if (!customers.Any(c => c.Id == customerId)) { Console.WriteLine("Customer not found."); return; }

    var order = new Order(customerId) { Id = nextOrderId };
    var addMore = true;

    while (addMore)
    {
        ListProducts();
        var productId = ReadInt("Product ID: ");
        if (!products.Any(p => p.Id == productId))
        {
            Console.WriteLine("Product not found.");
        }
        else
        {
            var quantity = ReadInt("Quantity: ", 1, 1000);
            order.Items[productId] = order.Items.GetValueOrDefault(productId) + quantity;
            Console.WriteLine("Product added to the order.");
        }
        addMore = Confirm("Add another product?");
    }

    if (order.Items.Count == 0) { Console.WriteLine("Empty order, it was not saved."); return; }

    orders.Add(order);
    nextOrderId++;
    Console.WriteLine("Order created:");
    ShowOrder(order);
}

void ListOrders()
{
    if (orders.Count == 0) { Console.WriteLine("There are no registered orders."); return; }
    foreach (var order in orders) { ShowOrder(order); Console.WriteLine(); }
}

void SearchOrder()
{
    Console.WriteLine("Search by: 1. ID  2. Customer name  3. Status");
    List<Order> results = [];

    switch (ReadInt("Criteria: ", 1, 3))
    {
        case 1:
            var id = ReadInt("ID: ");
            results = [.. orders.Where(o => o.Id == id)];
            break;
        case 2:
            var name = ReadText("Customer name contains: ");
            results = [.. orders.Where(o => CustomerName(o.CustomerId).Contains(name, StringComparison.OrdinalIgnoreCase))];
            break;
        case 3:
            Console.WriteLine("1. Pending  2. Delivered");
            var delivered = ReadInt("Status: ", 1, 2) == 2;
            results = [.. orders.Where(o => o.Delivered == delivered)];
            break;
    }

    if (results.Count == 0) Console.WriteLine("No results.");
    foreach (var order in results) { ShowOrder(order); Console.WriteLine(); }
}

Order? AskForOrder()
{
    var id = ReadInt("Order ID: ");
    var order = orders.FirstOrDefault(o => o.Id == id);
    if (order is null) Console.WriteLine("Order not found.");
    return order;
}

void EditOrder()
{
    if (AskForOrder() is not { } order) return;
    if (order.Delivered) { Console.WriteLine("A delivered order cannot be edited."); return; }

    ShowOrder(order);
    var productId = ReadInt("ID of the product to edit/add: ");
    if (!products.Any(p => p.Id == productId)) { Console.WriteLine("Product not found."); return; }

    var quantity = ReadInt("New quantity (0 to remove it from the order): ", 0, 1000);
    if (quantity == 0)
    {
        if (order.Items.Count == 1) { Console.WriteLine("The order cannot be left empty. Delete it instead if needed."); return; }
        order.Items.Remove(productId);
    }
    else
    {
        order.Items[productId] = quantity;
    }

    Console.WriteLine("Order updated:");
    ShowOrder(order);
}

void MarkAsDelivered()
{
    if (AskForOrder() is not { } order) return;
    if (order.Delivered) { Console.WriteLine("This order was already delivered."); return; }
    order.Delivered = true;
    Console.WriteLine("Order marked as delivered.");
}

void DeleteOrder()
{
    if (AskForOrder() is not { } order) return;
    orders.Remove(order);
    Console.WriteLine("Order deleted.");
}

// ===================== MODELS =====================
class Product(string name, string category, decimal price)
{
    public int Id { get; set; }
    public string Name { get; set; } = name;
    public string Category { get; set; } = category;
    public decimal Price { get; set; } = price;

    public override string ToString() => $"[{Id}] {Name} | {Category} | RD$ {Price:N2}";
}

class Customer(string name, string phone)
{
    public int Id { get; set; }
    public string Name { get; set; } = name;
    public string Phone { get; set; } = phone;

    public override string ToString() => $"[{Id}] {Name} | Phone: {Phone}";
}

class Order(int customerId)
{
    public int Id { get; set; }
    public int CustomerId { get; } = customerId;
    public DateTime Date { get; } = DateTime.Now;
    public bool Delivered { get; set; }
    // Dictionary: ProductId -> Quantity
    public Dictionary<int, int> Items { get; } = new();
}
