using SourceGeneratorLibrary.Api.Model;
using SourceGeneratorLibrary;
using System.Text.Json;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

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
    //var productResponse = new ProductResponse();
    return Results.Ok();
});

app.MapGet("/test", () =>
{
    var classNames = ClassNames.Names;
    return classNames;
});

app.MapGet("/spec", async () =>
{
    var path = Directory.GetCurrentDirectory();

    var spec = File.ReadAllText(Path.Combine(path, "spec.json"));

    var schema = JsonSerializer.Deserialize<Schema>(spec, new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    });

    if (schema is null)
    {
        throw new Exception("Schema is null");
    }

    var classDeclarations = schema.Types.Select(t => CreateClass(t.TypeName, t.Properties))
        .ToArray() ?? [];

    var compilationUnit = SyntaxFactory.FileScopedNamespaceDeclaration(SyntaxFactory.IdentifierName("ClasGen"))
        .AddMembers(classDeclarations);

    await using var streamWriter = new StreamWriter(Path.Combine(path, "ClassGen.cs"), false);

    compilationUnit.NormalizeWhitespace().WriteTo(streamWriter);

    return Results.Ok();
});

await app.RunAsync();

static ClassDeclarationSyntax CreateClass(
    string className,
    IReadOnlyCollection<PropertySchema> properties)
{
    var classDeclaration = SyntaxFactory.ClassDeclaration(
        SyntaxFactory.Identifier(className))
        .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

    foreach (var property in properties)
    {
        classDeclaration = classDeclaration.AddMembers(
            SyntaxFactory.PropertyDeclaration(
                SyntaxFactory.IdentifierName(property.Type),
                SyntaxFactory.Identifier(property.Name))
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddAccessorListAccessors(
                SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)))
            );
    }

    return classDeclaration;
}

public class Foo
{ }

public class Schema
{
    public IReadOnlyCollection<ClassSchema>? Types { get; set; } = [];
}

public class ClassSchema
{
    public string TypeName { get; set; }

    public IReadOnlyCollection<PropertySchema> Properties { get; set; }
}

public class PropertySchema
{
    public string Name { get; set; }

    public string Type { get; set; }
}