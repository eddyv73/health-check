using Orders.Api.Abstractions;
using Orders.Api.Domain;
using Orders.Api.Exceptions;

namespace Orders.Api.Services;

public sealed class OrderService(IOrderRepository repository) : IOrderService
{
    public IReadOnlyCollection<Order> GetAll() => repository.GetAll().OrderBy(x => x.CreatedAt).ToList();

    public Order GetById(Guid id)
    {
        return repository.GetById(id) ?? throw new OrderNotFoundException(id);
    }

    public Order Create(CreateOrderRequest request)
    {
        Validate(request.CustomerName, request.Amount);

        var order = new Order(
            Guid.NewGuid(),
            request.CustomerName.Trim(),
            request.Amount,
            DateTimeOffset.UtcNow);

        return repository.Add(order);
    }

    public Order Update(Guid id, UpdateOrderRequest request)
    {
        Validate(request.CustomerName, request.Amount);

        var existing = repository.GetById(id) ?? throw new OrderNotFoundException(id);
        var updated = existing with
        {
            CustomerName = request.CustomerName.Trim(),
            Amount = request.Amount
        };

        return repository.Update(updated);
    }

    public void Delete(Guid id)
    {
        var deleted = repository.Delete(id);
        if (!deleted)
        {
            throw new OrderNotFoundException(id);
        }
    }

    private static void Validate(string customerName, decimal amount)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(customerName))
        {
            errors[nameof(customerName)] = ["Customer name is required."];
        }

        if (amount <= 0)
        {
            errors[nameof(amount)] = ["Amount must be greater than zero."];
        }

        if (errors.Count > 0)
        {
            throw new InputValidationException(errors);
        }
    }
}
