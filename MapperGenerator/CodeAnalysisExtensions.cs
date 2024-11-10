using Microsoft.CodeAnalysis;

using System.Diagnostics;

namespace MapperGenerator;

public static partial class CodeAnalysisExtensions
{
    public static string FullTypeName(this ITypeSymbol typeSymbol) =>
        typeSymbol.ToDisplayString(new SymbolDisplayFormat(
            SymbolDisplayGlobalNamespaceStyle.Omitted,
            SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            SymbolDisplayGenericsOptions.IncludeTypeParameters,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.ExpandNullable
        ));
    public static bool IsNullable(this ITypeSymbol typeSymbol) =>
        typeSymbol.NullableAnnotation == NullableAnnotation.Annotated;

    public static bool IsNullableValueType(this ITypeSymbol typeSymbol) =>
        typeSymbol.IsValueType && typeSymbol.IsNullable();

    public static bool TryGetNullableValueUnderlyingType(this ITypeSymbol typeSymbol, out ITypeSymbol? underlyingType)
    {
        if (typeSymbol is INamedTypeSymbol namedType && typeSymbol.IsNullableValueType() && namedType.IsGenericType)
        {
            var typeParameters = namedType.TypeArguments;
            // Assert the generic is named System.Nullable<T> as expected.
            Debug.Assert(namedType.ConstructUnboundGenericType() is { } genericType && genericType.Name == "Nullable" && genericType.ContainingNamespace.Name == "System" && genericType.TypeArguments.Length == 1);
            Debug.Assert(typeParameters.Length == 1);
            underlyingType = typeParameters[0];
            // TODO: decide what to return when the underlying type is not declared due to some compilation error.
            // TypeKind.Error indicats a compilation error, specifically a nullable type where the underlying type was not found.
            // I have observed that IsValueType will be true in such cases even though it is actually unknown whether the missing type is a value type
            // I chose to return false but you may prefer something else. 
            return underlyingType.TypeKind == TypeKind.Error ? false : true;
        }
        underlyingType = null;
        return false;
    }

    public static bool IsEnum(this ITypeSymbol typeSymbol) =>
        typeSymbol is INamedTypeSymbol namedType && namedType.EnumUnderlyingType != null;

    public static bool IsNullableEnumType(this ITypeSymbol typeSymbol) =>
        typeSymbol.TryGetNullableValueUnderlyingType(out var underlyingType) == true && underlyingType.IsEnum();
}