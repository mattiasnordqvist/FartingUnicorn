using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
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
            sb.AppendLine();
            sb.AppendLine("namespace FartingUnicorn.Generated;");
            sb.AppendLine();

            sb.AppendLine($"public static partial class Mappers");
            using (var _1 = sb.CodeBlock())
            {
                sb.AppendLine($"public static Result<{classToGenerateMapperFor.FullName}> MapTo{classToGenerateMapperFor.FullName.Replace(".", "_")}(JsonElement jsonElement, string[] path = null)");
                using (var _2 = sb.CodeBlock())
                {
                    sb.AppendLine("if(path is null)");
                    using(var _3 = sb.CodeBlock())
                    {
                        sb.AppendLine("path = [\"$\"];");
                    }

                    sb.AppendLine("/*object*/");
                    using (var _3 = sb.CodeBlock())
                    {
                        sb.AppendLine("if (jsonElement.ValueKind != JsonValueKind.Object)");
                        using (var _4 = sb.CodeBlock())
                        {
                            sb.AppendLine($"return Result<{classToGenerateMapperFor.FullName}>.Error(new ValueHasWrongTypeError(path, \"Object\", jsonElement.ValueKind.ToString()));");
                        }
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
                            sb.AppendLine($"// {p.Type}, isOption = {p.IsOption}");
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

                            if (p.Type == "String")
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
                        }
                        sb.AppendLine("else");
                        using (var _3 = sb.CodeBlock())
                        {
                            //if nullable
                            //sb.AppendLine($"obj.{p.Name} = null;");

                            sb.AppendLine($"errors.Add(new RequiredPropertyMissingError([.. path, \"{p.Name}\"]));");
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
                // generate the source code and add it to the output
                var sourceText = SourceGenerationHelper.GenerateExtensionClass(value);
                // Create a separate partial class file for each enum
                context.AddSource($"Mapper.{value.FullName}.g.cs", sourceText);
            }
        }
        static bool IsSyntaxTargetForGeneration(SyntaxNode node)
            => node is ClassDeclarationSyntax m && m.AttributeLists.Count > 0;

        static ClassToGenerateMapperFor? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
        {
            // we know the node is a EnumDeclarationSyntax thanks to IsSyntaxTargetForGeneration
            var classDeclarationSyntaxNode = (ClassDeclarationSyntax)context.Node;

            // loop through all the attributes on the method
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

                    // Is the attribute the [EnumExtensions] attribute?
                    if (fullName == "DotNetThoughts.FartingUnicorn.CreateMapperAttribute")
                    {
                        // return the enum. Implementation shown in section 7.
                        return GetClassToGenerate(context.SemanticModel, classDeclarationSyntaxNode);
                    }
                }
            }

            // we didn't find the attribute we were looking for
            return null;
        }

        static ClassToGenerateMapperFor? GetClassToGenerate(SemanticModel semanticModel, SyntaxNode enumDeclarationSyntax)
        {
            // Get the semantic representation of the enum syntax
            if (semanticModel.GetDeclaredSymbol(enumDeclarationSyntax) is not INamedTypeSymbol classSymbol)
            {
                // something went wrong
                return null;
            }

            // Get the full type name of the enum e.g. Colour, 
            // or OuterClass<T>.Colour if it was nested in a generic type (for example)
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
                        ? p.Type.Name
                        : ((INamedTypeSymbol)p.Type).TypeArguments.First().Name;
                    properties.Add(new PropertyToGenerateMapperFor(p.Name, tName, isOptions));
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
    public PropertyToGenerateMapperFor(string name, string type, bool isOption)
    {
        Name = name;
        Type = type;
        IsOption = isOption;
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

public readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>, IEnumerable<T>
    where T : IEquatable<T>
{
    /// <summary>
    /// The underlying <typeparamref name="T"/> array.
    /// </summary>
    private readonly T[]? _array;

    /// <summary>
    /// Initializes a new instance of the <see cref="EquatableArray{T}"/> struct.
    /// </summary>
    /// <param name="array">The input array to wrap.</param>
    public EquatableArray(T[] array)
    {
        _array = array;
    }

    /// <summary>
    /// Gets the length of the array, or 0 if the array is null
    /// </summary>
    public int Count => _array?.Length ?? 0;

    /// <summary>
    /// Checks whether two <see cref="EquatableArray{T}"/> values are the same.
    /// </summary>
    /// <param name="left">The first <see cref="EquatableArray{T}"/> value.</param>
    /// <param name="right">The second <see cref="EquatableArray{T}"/> value.</param>
    /// <returns>Whether <paramref name="left"/> and <paramref name="right"/> are equal.</returns>
    public static bool operator ==(EquatableArray<T> left, EquatableArray<T> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Checks whether two <see cref="EquatableArray{T}"/> values are not the same.
    /// </summary>
    /// <param name="left">The first <see cref="EquatableArray{T}"/> value.</param>
    /// <param name="right">The second <see cref="EquatableArray{T}"/> value.</param>
    /// <returns>Whether <paramref name="left"/> and <paramref name="right"/> are not equal.</returns>
    public static bool operator !=(EquatableArray<T> left, EquatableArray<T> right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc/>
    public bool Equals(EquatableArray<T> array)
    {
        return AsSpan().SequenceEqual(array.AsSpan());
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is EquatableArray<T> array && Equals(this, array);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        if (_array is not T[] array)
        {
            return 0;
        }

        return ((IStructuralEquatable)_array).GetHashCode(EqualityComparer<int>.Default);

    }

    /// <summary>
    /// Returns a <see cref="ReadOnlySpan{T}"/> wrapping the current items.
    /// </summary>
    /// <returns>A <see cref="ReadOnlySpan{T}"/> wrapping the current items.</returns>
    public ReadOnlySpan<T> AsSpan()
    {
        return _array.AsSpan();
    }

    /// <summary>
    /// Returns the underlying wrapped array.
    /// </summary>
    /// <returns>Returns the underlying array.</returns>
    public T[]? AsArray()
    {
        return _array;
    }

    /// <inheritdoc/>
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return ((IEnumerable<T>)(_array ?? Array.Empty<T>())).GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<T>)(_array ?? Array.Empty<T>())).GetEnumerator();
    }
}
