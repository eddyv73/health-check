using OrdersApi.Models;

namespace OrdersApi.Services;

public interface IOrderService
{
    IEnumerable<Order> GetAll();
    Order? GetById(Guid id);
    Order Create(CreateOrderRequest request);
    Order? Update(Guid id, UpdateOrderRequest request);
    bool Delete(Guid id);
}
