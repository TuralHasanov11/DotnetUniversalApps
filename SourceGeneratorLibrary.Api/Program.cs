using SourceGeneratorLibrary.Api.Model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/products", (Product product) =>
{
    var productResponse = new ProductResponse();
    return Results.Ok();
});

await app.RunAsync();