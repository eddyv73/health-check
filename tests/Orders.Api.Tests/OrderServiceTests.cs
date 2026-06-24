using Orders.Api.Domain;
using Orders.Api.Exceptions;
using Orders.Api.Repositories;
using Orders.Api.Services;

namespace Orders.Api.Tests;

public sealed class OrderServiceTests
{
    private readonly OrderService _service = new(new InMemoryOrderRepository());

    [Fact]
    public void Create_WithValidInput_ShouldStoreOrder()
    {
        var order = _service.Create(new CreateOrderRequest("Acme", 150.50m));

        var saved = _service.GetById(order.Id);

        Assert.Equal("Acme", saved.CustomerName);
        Assert.Equal(150.50m, saved.Amount);
        Assert.NotEqual(Guid.Empty, saved.Id);
    }

    [Fact]
    public void Create_WithInvalidAmount_ShouldThrowValidationException()
    {
        var exception = Assert.Throws<InputValidationException>(() =>
            _service.Create(new CreateOrderRequest("Acme", 0)));

        Assert.True(exception.Errors.ContainsKey("amount"));
    }

    [Fact]
    public void Update_WithUnknownOrder_ShouldThrowNotFoundException()
    {
        Assert.Throws<OrderNotFoundException>(() =>
            _service.Update(Guid.NewGuid(), new UpdateOrderRequest("Acme", 10)));
    }
}
