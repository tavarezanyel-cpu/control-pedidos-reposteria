using BakeryOrderControl.Exceptions;

namespace BakeryOrderControl.Models;

public class Order(int customerId) : BaseEntity
{
    // Dictionary: ProductId -> Quantity (encapsulated, only changed through methods)
    private readonly Dictionary<int, int> _items = new();

    public int CustomerId { get; } = customerId;
    public DateTime Date { get; } = DateTime.Now;
    public bool Delivered { get; private set; }
    public IReadOnlyDictionary<int, int> Items => _items;

    /// <summary>Sets the quantity of a product. With 0 it removes the product from the order.</summary>
    public void SetQuantity(int productId, int quantity)
    {
        if (Delivered)
            throw new BakeryException("A delivered order cannot be modified.");
        if (quantity < 0)
            throw new BakeryException("The quantity cannot be negative.");

        if (quantity == 0)
        {
            if (!_items.ContainsKey(productId)) return;
            if (_items.Count == 1)
                throw new BakeryException("The order cannot be left empty. Delete the order instead.");
            _items.Remove(productId);
        }
        else
        {
            _items[productId] = quantity;
        }
    }

    public void MarkAsDelivered()
    {
        if (Delivered)
            throw new BakeryException("This order was already delivered.");
        if (_items.Count == 0)
            throw new BakeryException("An empty order cannot be delivered.");
        Delivered = true;
    }

    public override string Describe() =>
        $"Order #{Id} | Customer #{CustomerId} | {Date:dd/MM/yyyy HH:mm} | {(Delivered ? "DELIVERED" : "PENDING")}";
}
