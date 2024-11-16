using SampleDataGeneration.Api.Features.Orders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddTransient<OrderFaker>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => "Faker");

app.MapGet("/api/orders", (OrderFaker orderFaker) =>
{
    var orders = orderFaker.GetOrderGenerator().Generate(10);
    return TypedResults.Ok(orders);
})
.WithName("GetWeatherForecast");

await app.RunAsync();