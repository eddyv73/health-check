namespace OrdersApi.Models;

public record Order(Guid Id, string CustomerName, decimal Amount, DateTimeOffset CreatedAt);

public record CreateOrderRequest(string CustomerName, decimal Amount);

public record UpdateOrderRequest(string CustomerName, decimal Amount);
