namespace Orders.Api.Domain;

public record CreateOrderRequest(string CustomerName, decimal Amount);

public record UpdateOrderRequest(string CustomerName, decimal Amount);
