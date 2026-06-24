using OrdersApi.Models;
using OrdersApi.Repositories;

namespace OrdersApi.Services;

public sealed class OrderService(IOrderRepository repository) : IOrderService
{
    public IEnumerable<Order> GetAll() => repository.GetAll();

    public Order? GetById(Guid id) => repository.GetById(id);

    public Order Create(CreateOrderRequest request)
    {
        ValidateRequest(request.CustomerName, request.Amount);
        return repository.Create(request);
    }

    public Order? Update(Guid id, UpdateOrderRequest request)
    {
        ValidateRequest(request.CustomerName, request.Amount);
        return repository.Update(id, request);
    }

    public bool Delete(Guid id) => repository.Delete(id);

    private static void ValidateRequest(string customerName, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("CustomerName is required.", nameof(customerName));

        if (customerName.Length > 200)
            throw new ArgumentException("CustomerName must not exceed 200 characters.", nameof(customerName));

        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
    }
}
