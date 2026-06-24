using Orders.Api.Abstractions;
using Orders.Api.Domain;
using Orders.Api.ErrorHandling;
using Orders.Api.Repositories;
using Orders.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
builder.Services.AddSingleton<IOrderService, OrderService>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
app.MapGet("/ready", () => Results.Ok(new { status = "ready" }));

var orders = app.MapGroup("/orders");

orders.MapGet(string.Empty, (IOrderService service) => Results.Ok(service.GetAll()));

orders.MapGet("/{id:guid}", (Guid id, IOrderService service) =>
{
    var order = service.GetById(id);
    return Results.Ok(order);
});

orders.MapPost(string.Empty, (CreateOrderRequest request, IOrderService service) =>
{
    var order = service.Create(request);
    return Results.Created($"/orders/{order.Id}", order);
});

orders.MapPut("/{id:guid}", (Guid id, UpdateOrderRequest request, IOrderService service) =>
{
    var order = service.Update(id, request);
    return Results.Ok(order);
});

orders.MapDelete("/{id:guid}", (Guid id, IOrderService service) =>
{
    service.Delete(id);
    return Results.NoContent();
});

app.Run();
