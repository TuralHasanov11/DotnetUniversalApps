using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

namespace SourceGeneratorLibrary;

[Generator]
public class ClassNameGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Create a syntax provider to process class declarations
        var provider = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: (c, _) => c is ClassDeclarationSyntax, // Only process class declarations
                transform: (c, _) => (ClassDeclarationSyntax)c.Node)
            .Where(c => c is not null);

        // Combine all the class declarations
        var compilation = context.CompilationProvider.Combine(provider.Collect());

        // Register the source output
        context.RegisterSourceOutput(
            compilation,
            (context, source) => Execute(context, source.Left, source.Right));
    }

    private void Execute(
        SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax> typeList)
    {
        //if (!Debugger.IsAttached) Debugger.Launch();

        var stringBuilder = new StringBuilder();

        if (typeList.Length == 0)
        {
            var descriptor = new DiagnosticDescriptor(
                "SG0001",
                "No classes found",
                "No classes found in the compilation",
                "Problem",
                DiagnosticSeverity.Warning,
                true);

            context.ReportDiagnostic(Diagnostic.Create(descriptor, Location.None));
        }
        else
        {
            foreach (var item in typeList)
            {
                var symbol = compilation
                    .GetSemanticModel(item.SyntaxTree)
                    .GetDeclaredSymbol(item) as INamedTypeSymbol;

                if (symbol is not null)
                {
                    stringBuilder.AppendLine($"     \"{symbol.ToDisplayString()}\",");
                }
            }

            if (stringBuilder.Length > 0)
            {
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }

            // Define the code to be generated
            var code = $$"""
            namespace SourceGeneratorLibrary
            {
                public static class ClassNames
                {
                    public static List<string> Names = [
                        {{stringBuilder.ToString()}}
                    ];
                }
            }
            """;

            // Add the generated source code to the compilation
            context.AddSource("ClassNames.g.cs", code);
        }
    }
}