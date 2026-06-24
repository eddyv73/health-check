using Microsoft.AspNetCore.Diagnostics;
using OrdersApi.Models;
using OrdersApi.Repositories;
using OrdersApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
builder.Services.AddSingleton<IOrderService, OrderService>();
builder.Services.AddHealthChecks();
builder.Services.AddProblemDetails();

// Native OpenAPI support — only exposed in Development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddOpenApi();
}

var app = builder.Build();

// Exception handler that returns ProblemDetails for unhandled errors
app.UseExceptionHandler(exceptionApp =>
{
    exceptionApp.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerFeature>();
        var ex = feature?.Error;

        var (statusCode, title, detail) = ex switch
        {
            ArgumentException ae => (StatusCodes.Status422UnprocessableEntity, "Validation error", ae.Message),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred", ex?.Message ?? string.Empty)
        };

        context.Response.StatusCode = statusCode;
        await Results.Problem(title: title, detail: detail, statusCode: statusCode)
            .ExecuteAsync(context);
    });
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// --- Health probes ---
app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTimeOffset.UtcNow }))
    .WithName("Liveness")
    .WithTags("Health");

app.MapGet("/ready", () => Results.Ok(new { status = "Ready", timestamp = DateTimeOffset.UtcNow }))
    .WithName("Readiness")
    .WithTags("Health");

// --- Orders CRUD ---
var orders = app.MapGroup("/orders").WithTags("Orders");

orders.MapGet("/", (IOrderService svc) => Results.Ok(svc.GetAll()))
    .WithName("GetOrders");

orders.MapGet("/{id:guid}", (Guid id, IOrderService svc) =>
    svc.GetById(id) is Order order
        ? Results.Ok(order)
        : Results.NotFound(new { message = $"Order {id} not found." }))
    .WithName("GetOrderById");

orders.MapPost("/", (CreateOrderRequest request, IOrderService svc) =>
{
    if (string.IsNullOrWhiteSpace(request.CustomerName) || request.Amount <= 0)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(request.CustomerName))
            errors["CustomerName"] = ["CustomerName is required."];
        if (request.Amount <= 0)
            errors["Amount"] = ["Amount must be greater than zero."];

        return Results.ValidationProblem(errors);
    }

    var order = svc.Create(request);
    return Results.Created($"/orders/{order.Id}", order);
})
.WithName("CreateOrder");

orders.MapPut("/{id:guid}", (Guid id, UpdateOrderRequest request, IOrderService svc) =>
{
    if (string.IsNullOrWhiteSpace(request.CustomerName) || request.Amount <= 0)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(request.CustomerName))
            errors["CustomerName"] = ["CustomerName is required."];
        if (request.Amount <= 0)
            errors["Amount"] = ["Amount must be greater than zero."];

        return Results.ValidationProblem(errors);
    }

    return svc.Update(id, request) is Order updated
        ? Results.Ok(updated)
        : Results.NotFound(new { message = $"Order {id} not found." });
})
.WithName("UpdateOrder");

orders.MapDelete("/{id:guid}", (Guid id, IOrderService svc) =>
    svc.Delete(id)
        ? Results.NoContent()
        : Results.NotFound(new { message = $"Order {id} not found." }))
.WithName("DeleteOrder");

app.Run();

// Needed for integration test project accessibility
public partial class Program { }
