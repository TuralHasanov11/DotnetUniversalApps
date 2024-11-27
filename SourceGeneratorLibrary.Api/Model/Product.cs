namespace SourceGeneratorLibrary.Api.Model;

public class Product
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    //[MappableIgnore]
    public decimal Price { get; set; }
}