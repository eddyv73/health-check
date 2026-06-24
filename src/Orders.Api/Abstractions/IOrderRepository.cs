using Orders.Api.Domain;

namespace Orders.Api.Abstractions;

public interface IOrderRepository
{
    IReadOnlyCollection<Order> GetAll();
    Order? GetById(Guid id);
    Order Add(Order order);
    Order Update(Order order);
    bool Delete(Guid id);
}
