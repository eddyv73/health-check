using System.Collections.Concurrent;
using Orders.Api.Abstractions;
using Orders.Api.Domain;

namespace Orders.Api.Repositories;

public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, Order> _orders = new();

    public IReadOnlyCollection<Order> GetAll() => _orders.Values.ToList();

    public Order? GetById(Guid id) => _orders.TryGetValue(id, out var order) ? order : null;

    public Order Add(Order order)
    {
        _orders[order.Id] = order;
        return order;
    }

    public Order Update(Order order)
    {
        _orders[order.Id] = order;
        return order;
    }

    public bool Delete(Guid id) => _orders.TryRemove(id, out _);
}
