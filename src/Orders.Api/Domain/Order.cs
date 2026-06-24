namespace Orders.Api.Domain;

public record Order(Guid Id, string CustomerName, decimal Amount, DateTimeOffset CreatedAt);
