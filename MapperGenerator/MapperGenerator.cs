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
        public static string GenerateExtensionClass(ClassToGenerateMapperFor classToGenerateMapperFor)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using DotNetThoughts.Results;");
            sb.AppendLine("using System.Text.Json;");
            sb.AppendLine("namespace DotNetThoughts.FartingUnicorn;");
            sb.AppendLine($"public static partial class Mappers");
            sb.AppendLine("{");
            sb.AppendLine($"    public static Result<{classToGenerateMapperFor.Name}> Map(JsonElement jsonElement)");
            sb.AppendLine("    {");
            sb.AppendLine($"        var result = new {classToGenerateMapperFor.Name}();");
            foreach (var value in classToGenerateMapperFor.Values)
            {
                sb.AppendLine($"    result.{value} = jsonElement.GetProperty(\"{value}\").GetString();");
            }
            sb.AppendLine($"        return Result<{classToGenerateMapperFor.Name}>.Ok(result);");
            sb.AppendLine("     }");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }

    //public static Result<T> Map<T>(JsonElement json)
    //{
    //    Result<Unit> result = UnitResult.Ok;
    //    var userProfile = new UserProfile();


    //    // required property in all senses. It must be present in the JSON and must have a value (not null in json)
    //    if (!json.TryGetProperty("name", out var nameProperty))
    //    {
    //        // This handles the case when the property is completely missing
    //        result = result.Or(Result<Unit>.Error(new RequiredPropertyMissingError("name")));
    //    }
    //    else
    //    {
    //        if (nameProperty.ValueKind == JsonValueKind.Null)
    //        {
    //            result = result.Or(Result<Unit>.Error(new RequiredValueMissingError("name")));
    //        }
    //        userProfile.Name = nameProperty.GetString();
    //    }
    //    userProfile.Age = json.GetProperty("age").GetInt32();
    //    userProfile.IsSubscribed = json.GetProperty("isSubscribed").GetBoolean();
    //    userProfile.Courses = json.GetProperty("courses").EnumerateArray().Select(course => course.GetString()).ToArray();

    //    if (json.GetProperty("pet").ValueKind == JsonValueKind.Null)
    //    {
    //        userProfile.Pet = new None<Pet>();
    //    }
    //    else
    //    {
    //        userProfile.Pet = new Some<Pet>(new Pet
    //        {
    //            Name = json.GetProperty("pet").GetProperty("name").GetString(),
    //            Type = json.GetProperty("pet").GetProperty("type").GetString()
    //        });
    //    }

    //    if (json.TryGetProperty("isGay", out var isGay))
    //    {
    //        userProfile.IsGay = isGay.GetBoolean();
    //    }

    //    if (json.TryGetProperty("favoritePet", out var favoritePet))
    //    {
    //        userProfile.FavoritePet = new Pet
    //        {
    //            Name = favoritePet.GetProperty("name").GetString(),
    //            Type = favoritePet.GetProperty("type").GetString()
    //        };
    //    }

    //    return result.Map(() => userProfile);
    //}

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
        //context.RegisterSourceOutput(context.CompilationProvider, (sourceProductionContext, c) =>
        //{
        //    sourceProductionContext.AddSource("Mapper.g", @"public class Mapper {}");
        //});
        static void Execute(ClassToGenerateMapperFor? classToGenerateMapperFor, SourceProductionContext context)
        {
            if (classToGenerateMapperFor is { } value)
            {
                // generate the source code and add it to the output
                string result = SourceGenerationHelper.GenerateExtensionClass(value);
                // Create a separate partial class file for each enum
                context.AddSource($"Mapper.{value.Name}.g.cs", SourceText.From(result, Encoding.UTF8));
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
            if (semanticModel.GetDeclaredSymbol(enumDeclarationSyntax) is not INamedTypeSymbol enumSymbol)
            {
                // something went wrong
                return null;
            }

            // Get the full type name of the enum e.g. Colour, 
            // or OuterClass<T>.Colour if it was nested in a generic type (for example)
            string className = enumSymbol.ToString();

            // Get all the members in the enum
            ImmutableArray<ISymbol> enumMembers = enumSymbol.GetMembers();
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

            return new ClassToGenerateMapperFor(className, members);
        }
    }
}

public readonly record struct ClassToGenerateMapperFor
{
    public readonly string Name;
    public readonly EquatableArray<string> Values;

    public ClassToGenerateMapperFor(string name, List<string> values)
    {
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
