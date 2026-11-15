using BakeryOrderControl.Exceptions;
using BakeryOrderControl.Models;
using BakeryOrderControl.Repositories;

namespace BakeryOrderControl.Services;

public class CustomerService(IRepository<Customer> customers, IRepository<Order> orders)
{
    public Customer Add(string name, string phone)
    {
        var customer = new Customer(name, phone);
        customers.Add(customer);
        return customer;
    }

    public IReadOnlyList<Customer> GetAll() => customers.GetAll();

    public Customer? FindById(int id) => customers.GetById(id);

    public IReadOnlyList<Customer> SearchByName(string text) =>
        customers.Find(c => c.Name.Contains(text, StringComparison.OrdinalIgnoreCase));

    public IReadOnlyList<Customer> SearchByPhone(string text) =>
        customers.Find(c => c.Phone.Contains(text));

    public Customer Edit(int id, string? name, string? phone)
    {
        var customer = customers.GetById(id) ?? throw new BakeryException("Customer not found.");

        if (!string.IsNullOrWhiteSpace(name)) customer.Name = name;
        if (!string.IsNullOrWhiteSpace(phone)) customer.Phone = phone;

        customers.Update(customer);
        return customer;
    }

    public void Delete(int id)
    {
        if (customers.GetById(id) is null)
            throw new BakeryException("Customer not found.");
        if (orders.Find(o => o.CustomerId == id).Count > 0)
            throw new BakeryException("Cannot delete: the customer has registered orders.");
        customers.Delete(id);
    }
}
