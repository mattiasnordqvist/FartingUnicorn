using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using System.Collections;
using System.Collections.Immutable;
using System.Text;

namespace MapperGenerator;

[Generator]
public class MapperGenerator : IIncrementalGenerator
{
    public static class SourceGenerationHelper
    {
        public const string Attribute = @"
namespace DotNetThoughts.FartingUnicorn
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class CreateMapperAttribute : System.Attribute
    {
    }
}";
        public static SourceText GenerateExtensionClass(ClassToGenerateMapperFor classToGenerateMapperFor)
        {
            var sb = new SourceBuilder();
            sb.AppendLine("using DotNetThoughts.Results;");
            sb.AppendLine("using System.Text.Json;");
            sb.AppendLine();
            sb.AppendLine("namespace FartingUnicorn.Generated;");
            sb.AppendLine($"public static partial class Mappers");
            sb.AppendLine("{");
            using (var _ = sb.Indent())
            {
                sb.AppendLine($"public static Result<{classToGenerateMapperFor.FullName}> MapTo{classToGenerateMapperFor.Name}(JsonElement jsonElement)");
                sb.AppendLine("{");
                using (var __ = sb.Indent())
                {
                    sb.AppendLine($"var result = new {classToGenerateMapperFor.FullName}();");
                    foreach (var value in classToGenerateMapperFor.Values)
                    {
                        sb.AppendLine($"result.{value} = jsonElement.GetProperty(\"{value}\").GetString();");
                    }
                    sb.AppendLine($"return Result<{classToGenerateMapperFor.FullName}>.Ok(result);");

                }
                sb.AppendLine("}");
            }
            sb.AppendLine("}");
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
            ImmutableArray<ISymbol> enumMembers = classSymbol.GetMembers();
            var members = new List<string>(enumMembers.Length);

            // Get all the fields from the enum, and add their name to the list
            foreach (ISymbol member in enumMembers)
            {
                if (member is IFieldSymbol field && field.ConstantValue is not null)
                {
                    members.Add(member.Name);
                }
            }

            foreach (ISymbol member in enumMembers)
            {
                if (member is IFieldSymbol field && field.ConstantValue is not null)
                {
                    members.Add(member.Name);
                }
            }

            return new ClassToGenerateMapperFor(className, name, members);
        }
    }
}

public readonly record struct ClassToGenerateMapperFor
{
    public readonly string FullName;
    public readonly string Name;
    public readonly EquatableArray<string> Values;

    public ClassToGenerateMapperFor(string fullName, string name, List<string> values)
    {
        FullName = fullName;
        Name = name;
        Values = new(values.ToArray());
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
