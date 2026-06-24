using OrdersApi.Models;

namespace OrdersApi.Repositories;

public interface IOrderRepository
{
    IEnumerable<Order> GetAll();
    Order? GetById(Guid id);
    Order Create(CreateOrderRequest request);
    Order? Update(Guid id, UpdateOrderRequest request);
    bool Delete(Guid id);
}
