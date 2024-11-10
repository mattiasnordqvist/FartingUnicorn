using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using System.Collections.Immutable;
using System.Text;

namespace MapperGenerator;

[Generator]
public class MapperGenerator : IIncrementalGenerator
{
    public static class SourceGenerationHelper
    {
        public const string Attribute = """
            namespace DotNetThoughts.FartingUnicorn
            {
                [System.AttributeUsage(System.AttributeTargets.Class)]
                public class CreateMapperAttribute : System.Attribute
                {
                }
            }
            """;


        public static SourceText GenerateExtensionClass(ClassToGenerateMapperFor classToGenerateMapperFor)
        {
            var sb = new SourceBuilder();
            sb.AppendLine("using DotNetThoughts.Results;");
            sb.AppendLine("using System.Text.Json;");
            sb.AppendLine("using static FartingUnicorn.MapperOptions;");
            sb.AppendLine();
            sb.AppendLine("namespace FartingUnicorn.Generated;");
            sb.AppendLine();

            sb.AppendLine($"public static partial class Mappers");
            using (var _1 = sb.CodeBlock())
            {
                sb.AppendLine($"public static Result<{classToGenerateMapperFor.FullName}> MapTo{classToGenerateMapperFor.FullName.Replace(".", "_")}(JsonElement jsonElement, MapperOptions mapperOptions = null, string[] path = null)");
                using (var _2 = sb.CodeBlock())
                {
                    sb.AppendLine("if (mapperOptions is null)");
                    using (var _3 = sb.CodeBlock())
                    {
                        sb.AppendLine("mapperOptions = new MapperOptions();");
                    }

                    sb.AppendLine("if (path is null)");
                    using (var _3 = sb.CodeBlock())
                    {
                        sb.AppendLine("path = [\"$\"];");
                    }

                    sb.AppendLine("if (jsonElement.ValueKind != JsonValueKind.Object)");
                    using (var _4 = sb.CodeBlock())
                    {
                        sb.AppendLine($"return Result<{classToGenerateMapperFor.FullName}>.Error(new ValueHasWrongTypeError(path, \"Object\", jsonElement.ValueKind.ToString()));");
                    }

                    sb.AppendLine($"var obj = new {classToGenerateMapperFor.FullName}();");
                    sb.AppendLine();
                    sb.AppendLine("List<IError> errors = new();");

                    foreach (var p in classToGenerateMapperFor.Properties)
                    {
                        sb.AppendLine($"var is{p.Name}PropertyDefined = jsonElement.TryGetProperty(\"{p.Name}\", out var json{p.Name}Property);");
                        sb.AppendLine($"if (is{p.Name}PropertyDefined)");
                        using (var _3 = sb.CodeBlock())
                        {
                            sb.AppendLine($"// type = {p.Type}, isOption = {p.IsOption}, isNullable = {p.IsNullable}");
                            sb.AppendLine($"if (json{p.Name}Property.ValueKind == JsonValueKind.Null)");
                            using (var _4 = sb.CodeBlock())
                            {
                                if (p.IsOption)
                                {
                                    sb.AppendLine($"obj.{p.Name} = new None<{p.Type}>();");
                                }
                                else
                                {
                                    sb.AppendLine($"errors.Add(new RequiredValueMissingError([.. path, \"{p.Name}\"]));");
                                }
                            }
                            if (p.Type == "System.String")
                            {
                                sb.AppendLine($"else if (json{p.Name}Property.ValueKind == JsonValueKind.String)");
                                using (var _4 = sb.CodeBlock())
                                {
                                    if (p.IsOption)
                                    {
                                        sb.AppendLine($"obj.{p.Name} = new Some<string>(json{p.Name}Property.GetString());");

                                    }
                                    else
                                    {
                                        sb.AppendLine($"obj.{p.Name} = json{p.Name}Property.GetString();");
                                    }
                                }
                                sb.AppendLine("else");
                                using (var _4 = sb.CodeBlock())
                                {
                                    sb.AppendLine($"errors.Add(new ValueHasWrongTypeError([.. path, \"{p.Name}\"], \"String\", json{p.Name}Property.ValueKind.ToString()));");
                                }
                            }
                            else if(p.Type == "System.Boolean")
                            {
                                sb.AppendLine($"else if (json{p.Name}Property.ValueKind == JsonValueKind.True || json{p.Name}Property.ValueKind == JsonValueKind.False)");
                                using (var _4 = sb.CodeBlock())
                                {
                                    if (p.IsOption)
                                    {
                                        sb.AppendLine($"obj.{p.Name} = new Some<bool>(json{p.Name}Property.GetBoolean());");

                                    }
                                    else
                                    {
                                        sb.AppendLine($"obj.{p.Name} = json{p.Name}Property.GetBoolean();");
                                    }
                                }
                                sb.AppendLine("else");
                                using (var _4 = sb.CodeBlock())
                                {
                                    sb.AppendLine($"errors.Add(new ValueHasWrongTypeError([.. path, \"{p.Name}\"], \"Boolean\", json{p.Name}Property.ValueKind.ToString()));");
                                }
                            }
                            else if (p.Type == "System.Int32")
                            {
                                sb.AppendLine($"else if (json{p.Name}Property.ValueKind == JsonValueKind.Number)");
                                using (var _4 = sb.CodeBlock())
                                {
                                    if (p.IsOption)
                                    {
                                        sb.AppendLine($"obj.{p.Name} = new Some<int>(json{p.Name}Property.GetInt32());");

                                    }
                                    else
                                    {
                                        sb.AppendLine($"obj.{p.Name} = json{p.Name}Property.GetInt32();");
                                    }
                                }
                                sb.AppendLine("else");
                                using (var _4 = sb.CodeBlock())
                                {
                                    sb.AppendLine($"errors.Add(new ValueHasWrongTypeError([.. path, \"{p.Name}\"], \"Number\", json{p.Name}Property.ValueKind.ToString()));");
                                }
                            }
                            else // custom converter?
                            {
                                sb.AppendLine($"else if (mapperOptions.TryGetConverter(typeof({p.Type}), out IConverter customConverter))");
                                using(var _4 = sb.CodeBlock())
                                {
                                    sb.AppendLine($"if (json{p.Name}Property.ValueKind != customConverter.ExpectedJsonValueKind)");
                                    using(var _5 = sb.CodeBlock())
                                    {
                                        sb.AppendLine($"errors.Add(new ValueHasWrongTypeError([.. path, \"{p.Name}\"], customConverter.ExpectedJsonValueKind.ToString(), json{p.Name}Property.ValueKind.ToString()));");
                                    }
                                    sb.AppendLine("else");
                                    using (var _5 = sb.CodeBlock())
                                    {
                                        sb.AppendLine($"var result = customConverter.Convert(typeof({p.Type}), json{p.Name}Property, mapperOptions, [.. path, \"{p.Name}\"]);");
                                        sb.AppendLine("if (result.Success)");
                                        using (var _6 = sb.CodeBlock())
                                        {
                                            if(p.IsOption)
                                                sb.AppendLine($"obj.{p.Name} = new Some<{p.Type}>(result.Map(x => ({p.Type})x).Value);");
                                            else
                                                sb.AppendLine($"obj.{p.Name} = result.Map(x => ({p.Type})x).Value;");
                                        }
                                        sb.AppendLine("else");
                                        using (var _6 = sb.CodeBlock())
                                        {
                                            sb.AppendLine("errors.AddRange(result.Errors.Select(x => new MappingError([.. path, \"{p.Name}\"], x.Message)).ToArray());");
                                        }
                                    }
                                }
                            }
                        }
                        sb.AppendLine("else");
                        using (var _3 = sb.CodeBlock())
                        {
                            if (p.IsNullable)
                            {
                                sb.AppendLine($"obj.{p.Name} = null;");
                            }
                            else
                            {
                                sb.AppendLine($"errors.Add(new RequiredPropertyMissingError([.. path, \"{p.Name}\"]));");
                            }
                        }
                    }

                    sb.AppendLine("if(errors.Any())");
                    using (var _3 = sb.CodeBlock())
                    {
                        sb.AppendLine($"return Result<{classToGenerateMapperFor.FullName}>.Error(errors);");
                    }

                    sb.AppendLine("if(false)/*check if is option*/");
                    using (var _3 = sb.CodeBlock())
                    {
                        // handle if option
                    }
                    sb.AppendLine("else");
                    using (var _3 = sb.CodeBlock())
                    {
                        sb.AppendLine($"return Result<{classToGenerateMapperFor.FullName}>.Ok(obj);");
                    }
                    sb.AppendLine("throw new NotImplementedException();");

                }
            }

            return sb.ToSourceText();
        }
    }


    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Add the marker attribute to the compilation
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "CreateMapperAttribute.g.cs",
            SourceText.From(SourceGenerationHelper.Attribute, Encoding.UTF8)));

        // Do a simple filter for enums
        IncrementalValuesProvider<ClassToGenerateMapperFor?> classesToGenerate = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s), // select enums with attributes
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx)) // select enums with the [EnumExtensions] attribute and extract details
            .Where(static m => m is not null); // Filter out errors that we don't care about

        context.RegisterSourceOutput(classesToGenerate,
          static (spc, source) => Execute(source, spc));
        static void Execute(ClassToGenerateMapperFor? classToGenerateMapperFor, SourceProductionContext context)
        {
            if (classToGenerateMapperFor is { } value)
            {
                var sourceText = SourceGenerationHelper.GenerateExtensionClass(value);
                context.AddSource($"Mapper.{value.FullName}.g.cs", sourceText);
            }
        }
        static bool IsSyntaxTargetForGeneration(SyntaxNode node)
            => node is ClassDeclarationSyntax m && m.AttributeLists.Count > 0;

        static ClassToGenerateMapperFor? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
        {
            var classDeclarationSyntaxNode = (ClassDeclarationSyntax)context.Node;

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

                    if (fullName == "DotNetThoughts.FartingUnicorn.CreateMapperAttribute")
                    {
                        return GetClassToGenerate(context.SemanticModel, classDeclarationSyntaxNode);
                    }
                }
            }

            return null;
        }

        static ClassToGenerateMapperFor? GetClassToGenerate(SemanticModel semanticModel, SyntaxNode enumDeclarationSyntax)
        {
            if (semanticModel.GetDeclaredSymbol(enumDeclarationSyntax) is not INamedTypeSymbol classSymbol)
            {
                // something went wrong
                return null;
            }

            string className = classSymbol.ToString();
            string name = classSymbol.Name.ToString();

            // Get all the members in the enum
            ImmutableArray<ISymbol> members = classSymbol.GetMembers();
            var properties = new List<PropertyToGenerateMapperFor>(members.Length);

            // Get all the props from the class, and add their name to the list
            foreach (ISymbol member in members)
            {
                if (member is IPropertySymbol p)
                {
                    var t = p.Type;
                    var isOptions = t.Name == "Option";
                    var tName = !isOptions
                        ? p.Type.FullTypeName()
                        : ((INamedTypeSymbol)p.Type).TypeArguments.First().FullTypeName();

                    var isNullable = t.IsNullable();
                    if (t.IsNullableValueType())
                    {
                        tName = ((INamedTypeSymbol)p.Type).TypeArguments.First().FullTypeName();
                    }
                    properties.Add(new PropertyToGenerateMapperFor(p.Name, tName, isOptions, isNullable));
                }
            }

            return new ClassToGenerateMapperFor(className, name, properties);
        }
    }
}
public readonly record struct PropertyToGenerateMapperFor
{
    public readonly string Name;
    public readonly string Type;
    public readonly bool IsOption;
    public readonly bool IsNullable;
    public PropertyToGenerateMapperFor(string name, string type, bool isOption, bool isNullable)
    {
        Name = name;
        Type = type;
        IsOption = isOption;
        IsNullable = isNullable;
    }

}
public readonly record struct ClassToGenerateMapperFor
{
    public readonly string FullName;
    public readonly string Name;
    public readonly EquatableArray<PropertyToGenerateMapperFor> Properties;

    public ClassToGenerateMapperFor(string fullName, string name, List<PropertyToGenerateMapperFor> properties)
    {
        FullName = fullName;
        Name = name;
        Properties = new(properties.ToArray());
    }
}
