using System.Collections.Concurrent;
using OrdersApi.Models;

namespace OrdersApi.Repositories;

/// <summary>
/// Thread-safe in-memory implementation. Replace with EF Core for production.
/// </summary>
public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, Order> _store = new();

    public IEnumerable<Order> GetAll() => _store.Values.ToList();

    public Order? GetById(Guid id) =>
        _store.TryGetValue(id, out var order) ? order : null;

    public Order Create(CreateOrderRequest request)
    {
        var order = new Order(Guid.NewGuid(), request.CustomerName, request.Amount, DateTimeOffset.UtcNow);
        _store[order.Id] = order;
        return order;
    }

    public Order? Update(Guid id, UpdateOrderRequest request)
    {
        if (!_store.TryGetValue(id, out var existing))
            return null;

        var updated = existing with
        {
            CustomerName = request.CustomerName,
            Amount = request.Amount
        };
        _store[id] = updated;
        return updated;
    }

    public bool Delete(Guid id) => _store.TryRemove(id, out _);
}
