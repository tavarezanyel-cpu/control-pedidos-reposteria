using BakeryOrderControl.Exceptions;
using BakeryOrderControl.Models;
using BakeryOrderControl.Repositories;

namespace BakeryOrderControl.Services;

public class OrderService(
    IRepository<Order> orders,
    IRepository<Customer> customers,
    IRepository<Product> products)
{
    public Order Create(int customerId, IDictionary<int, int> items)
    {
        if (customers.GetById(customerId) is null)
            throw new BakeryException("Customer not found.");
        if (items.Count == 0)
            throw new BakeryException("The order must have at least one product.");

        var order = new Order(customerId);
        foreach (var (productId, quantity) in items)
        {
            ValidateProduct(productId);
            if (quantity <= 0)
                throw new BakeryException("Quantities must be greater than 0.");
            order.SetQuantity(productId, quantity);
        }

        orders.Add(order);
        return order;
    }

    public IReadOnlyList<Order> GetAll() => orders.GetAll();

    public Order? FindById(int id) => orders.GetById(id);

    public IReadOnlyList<Order> SearchByCustomerName(string text) =>
        orders.Find(o => CustomerName(o).Contains(text, StringComparison.OrdinalIgnoreCase));

    public IReadOnlyList<Order> SearchByStatus(bool delivered) =>
        orders.Find(o => o.Delivered == delivered);

    public Order ChangeQuantity(int orderId, int productId, int quantity)
    {
        var order = GetOrder(orderId);
        ValidateProduct(productId);
        order.SetQuantity(productId, quantity);
        orders.Update(order);
        return order;
    }

    public void MarkAsDelivered(int orderId)
    {
        var order = GetOrder(orderId);
        order.MarkAsDelivered();
        orders.Update(order);
    }

    public void Delete(int orderId)
    {
        GetOrder(orderId);
        orders.Delete(orderId);
    }

    public string CustomerName(Order order) =>
        customers.GetById(order.CustomerId)?.Name ?? "(unknown)";

    public decimal CalculateTotal(Order order) =>
        order.Items.Sum(i => (products.GetById(i.Key)?.Price ?? 0) * i.Value);

    public string DescribeDetails(Order order)
    {
        var lines = order.Items.Select(i =>
            $"    - {products.GetById(i.Key)?.Name ?? "(deleted)"} x{i.Value}");

        return $"{order.Describe()} ({CustomerName(order)})\n" +
               $"{string.Join('\n', lines)}\n" +
               $"    Total: RD$ {CalculateTotal(order):N2}";
    }

    private Order GetOrder(int id) =>
        orders.GetById(id) ?? throw new BakeryException("Order not found.");

    private void ValidateProduct(int productId)
    {
        if (products.GetById(productId) is null)
            throw new BakeryException($"Product #{productId} not found.");
    }
}
