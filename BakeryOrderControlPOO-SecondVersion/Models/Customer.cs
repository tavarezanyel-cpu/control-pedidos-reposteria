using BakeryOrderControl.Exceptions;

namespace BakeryOrderControl.Models;

public class Customer : BaseEntity
{
    private string _name = "";
    private string _phone = "";

    public string Name
    {
        get => _name;
        set => _name = string.IsNullOrWhiteSpace(value)
            ? throw new BakeryException("The customer name cannot be empty.")
            : value.Trim();
    }

    public string Phone
    {
        get => _phone;
        set => _phone = string.IsNullOrWhiteSpace(value)
            ? throw new BakeryException("The phone cannot be empty.")
            : value.Trim();
    }

    public Customer(string name, string phone)
    {
        Name = name;
        Phone = phone;
    }

    public override string Describe() => $"[{Id}] {Name} | Phone: {Phone}";
}
