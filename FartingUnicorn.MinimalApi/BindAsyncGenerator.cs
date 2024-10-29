using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using System.Collections.Immutable;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace MapperGenerator;

[Generator]
public class BindAsyncGenerator : IIncrementalGenerator
{
    public static class SourceGenerationHelper
    {
        public const string Attribute = @"
namespace DotNetThoughts.FartingUnicorn.MinimalApi
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class GenerateBindAsyncAttribute : System.Attribute
    {
    }
}";
        public static string GenerateExtensionClass(ClassToGenerateMapperFor classToGenerateMapperFor)
        {
            var sb = new StringBuilder();
            sb.AppendLine("/* auto-generated */");
            sb.AppendLine($"using DotNetThoughts.Results;");
            sb.AppendLine($"using FartingUnicorn;");
            sb.AppendLine();
            sb.AppendLine($"using System.Reflection;");
            sb.AppendLine($"using System.Text.Json;");
            sb.AppendLine($"");
            sb.AppendLine($"namespace {classToGenerateMapperFor.Ns};");

            sb.AppendLine($"public partial class {classToGenerateMapperFor.Name} {{");
            sb.AppendLine($"    public static async ValueTask<{classToGenerateMapperFor.Name}> BindAsync(HttpContext context, ParameterInfo parameter) {{");
            sb.AppendLine($"        if (!context.Request.HasJsonContentType())");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            throw new BadHttpRequestException(");
            sb.AppendLine($"                \"Request content type was not a recognized JSON content type.\",");
            sb.AppendLine($"                StatusCodes.Status415UnsupportedMediaType);");
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine($"        using var json = await JsonDocument.ParseAsync(context.Request.Body);");
            sb.AppendLine($"        var rootElement = json.RootElement;");
            sb.AppendLine($"        return Mapper.Map<{classToGenerateMapperFor.Name}>(rootElement).ValueOrThrow();");
            sb.AppendLine($"    }}");
            sb.AppendLine($"}}");

            return sb.ToString();
        }
    }


    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Add the marker attribute to the compilation
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "GenerateBindAsyncAttribute.g.cs",
            SourceText.From(SourceGenerationHelper.Attribute, Encoding.UTF8)));

        IncrementalValuesProvider<ClassToGenerateMapperFor?> classesToGenerate = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null); // Filter out errors that we don't care about

        context.RegisterSourceOutput(classesToGenerate,
          static (spc, source) => Execute(source, spc));

        static void Execute(ClassToGenerateMapperFor? classToGenerateBindAsyncFor, SourceProductionContext context)
        {
            if (classToGenerateBindAsyncFor is { } value)
            {
                string result = SourceGenerationHelper.GenerateExtensionClass(value);
                context.AddSource($"{value.Name}.BindAsync.g.cs", SourceText.From(result, Encoding.UTF8));
            }
        }
        static bool IsSyntaxTargetForGeneration(SyntaxNode node)
            => node is ClassDeclarationSyntax m && m.AttributeLists.Count > 0;

        static ClassToGenerateMapperFor? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
        {
            var classDeclarationSyntaxNode = (ClassDeclarationSyntax)context.Node;

            // loop through all the attributes on the class
            foreach (AttributeListSyntax attributeListSyntax in classDeclarationSyntaxNode.AttributeLists)
            {
                foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
                {
                    if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                    {
                        // weird, we couldn't get the symbol, ignore it
                        continue;
                    }

                    INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                    string fullName = attributeContainingTypeSymbol.ToDisplayString();

                    // Is the attribute the [GenerateBindAsyncAttribute] attribute?
                    if (fullName == "DotNetThoughts.FartingUnicorn.MinimalApi.GenerateBindAsyncAttribute")
                    {
                        // return the class
                        return GetClassToGenerate(context.SemanticModel, classDeclarationSyntaxNode);
                    }
                }
            }

            // we didn't find the attribute we were looking for
            return null;
        }

        static ClassToGenerateMapperFor? GetClassToGenerate(SemanticModel semanticModel, SyntaxNode classDeclarationSyntax)
        {
            if (semanticModel.GetDeclaredSymbol(classDeclarationSyntax) is not INamedTypeSymbol classSymbol)
            {
                // something went wrong
                return null;
            }

            string className = classSymbol.ToString().Split('.').Last();
            var ns = GetNamespace((BaseTypeDeclarationSyntax)classDeclarationSyntax);

            return new ClassToGenerateMapperFor(className, ns);
        }
    }

    // determine the namespace the class/enum/struct is declared in, if any
    static string GetNamespace(BaseTypeDeclarationSyntax syntax)
    {
        // If we don't have a namespace at all we'll return an empty string
        // This accounts for the "default namespace" case
        string nameSpace = string.Empty;

        // Get the containing syntax node for the type declaration
        // (could be a nested type, for example)
        SyntaxNode? potentialNamespaceParent = syntax.Parent;

        // Keep moving "out" of nested classes etc until we get to a namespace
        // or until we run out of parents
        while (potentialNamespaceParent != null &&
                potentialNamespaceParent is not NamespaceDeclarationSyntax
                && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
        {
            potentialNamespaceParent = potentialNamespaceParent.Parent;
        }

        // Build up the final namespace by looping until we no longer have a namespace declaration
        if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
        {
            // We have a namespace. Use that as the type
            nameSpace = namespaceParent.Name.ToString();

            // Keep moving "out" of the namespace declarations until we 
            // run out of nested namespace declarations
            while (true)
            {
                if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
                {
                    break;
                }

                // Add the outer namespace as a prefix to the final namespace
                nameSpace = $"{namespaceParent.Name}.{nameSpace}";
                namespaceParent = parent;
            }
        }

        // return the final namespace
        return nameSpace;
    }
}
