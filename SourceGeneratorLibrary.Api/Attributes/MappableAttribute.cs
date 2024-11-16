namespace SourceGeneratorLibrary.Api.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class MappableAttribute : Attribute
{
    public string Suffix { get; }

    public MappableAttribute(string suffix = "")
    {
        Suffix = suffix;
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class MappableIgnoreAttribute : Attribute
{
}