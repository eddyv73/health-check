using FluentAssertions;
using NSubstitute;
using OrdersApi.Models;
using OrdersApi.Repositories;
using OrdersApi.Services;

namespace OrdersApi.Tests;

public class OrderServiceTests
{
    private readonly IOrderRepository _repository;
    private readonly IOrderService _sut;

    public OrderServiceTests()
    {
        _repository = Substitute.For<IOrderRepository>();
        _sut = new OrderService(_repository);
    }

    [Fact]
    public void GetAll_ReturnsAllOrdersFromRepository()
    {
        var expected = new[]
        {
            new Order(Guid.NewGuid(), "Alice", 100m, DateTimeOffset.UtcNow),
            new Order(Guid.NewGuid(), "Bob", 200m, DateTimeOffset.UtcNow)
        };
        _repository.GetAll().Returns(expected);

        var result = _sut.GetAll();

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GetById_WhenOrderExists_ReturnsOrder()
    {
        var id = Guid.NewGuid();
        var order = new Order(id, "Alice", 99.99m, DateTimeOffset.UtcNow);
        _repository.GetById(id).Returns(order);

        var result = _sut.GetById(id);

        result.Should().Be(order);
    }

    [Fact]
    public void GetById_WhenOrderNotFound_ReturnsNull()
    {
        _repository.GetById(Arg.Any<Guid>()).Returns((Order?)null);

        var result = _sut.GetById(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public void Create_WithValidRequest_DelegatesToRepository()
    {
        var request = new CreateOrderRequest("Alice", 50m);
        var expected = new Order(Guid.NewGuid(), "Alice", 50m, DateTimeOffset.UtcNow);
        _repository.Create(request).Returns(expected);

        var result = _sut.Create(request);

        result.Should().Be(expected);
        _repository.Received(1).Create(request);
    }

    [Theory]
    [InlineData("", 100)]
    [InlineData("   ", 100)]
    public void Create_WithEmptyCustomerName_ThrowsArgumentException(string customerName, decimal amount)
    {
        var request = new CreateOrderRequest(customerName, amount);

        var act = () => _sut.Create(request);

        act.Should().Throw<ArgumentException>().WithParameterName("customerName");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-999.99)]
    public void Create_WithNonPositiveAmount_ThrowsArgumentException(decimal amount)
    {
        var request = new CreateOrderRequest("Alice", amount);

        var act = () => _sut.Create(request);

        act.Should().Throw<ArgumentException>().WithParameterName("amount");
    }

    [Fact]
    public void Update_WithValidRequest_DelegatesToRepository()
    {
        var id = Guid.NewGuid();
        var request = new UpdateOrderRequest("Bob", 250m);
        var updated = new Order(id, "Bob", 250m, DateTimeOffset.UtcNow);
        _repository.Update(id, request).Returns(updated);

        var result = _sut.Update(id, request);

        result.Should().Be(updated);
        _repository.Received(1).Update(id, request);
    }

    [Fact]
    public void Update_WhenOrderNotFound_ReturnsNull()
    {
        var request = new UpdateOrderRequest("Bob", 250m);
        _repository.Update(Arg.Any<Guid>(), Arg.Any<UpdateOrderRequest>()).Returns((Order?)null);

        var result = _sut.Update(Guid.NewGuid(), request);

        result.Should().BeNull();
    }

    [Fact]
    public void Delete_WhenOrderExists_ReturnsTrue()
    {
        var id = Guid.NewGuid();
        _repository.Delete(id).Returns(true);

        var result = _sut.Delete(id);

        result.Should().BeTrue();
    }

    [Fact]
    public void Delete_WhenOrderNotFound_ReturnsFalse()
    {
        _repository.Delete(Arg.Any<Guid>()).Returns(false);

        var result = _sut.Delete(Guid.NewGuid());

        result.Should().BeFalse();
    }
}
