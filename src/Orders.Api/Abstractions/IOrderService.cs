using Orders.Api.Domain;

namespace Orders.Api.Abstractions;

public interface IOrderService
{
    IReadOnlyCollection<Order> GetAll();
    Order GetById(Guid id);
    Order Create(CreateOrderRequest request);
    Order Update(Guid id, UpdateOrderRequest request);
    void Delete(Guid id);
}
