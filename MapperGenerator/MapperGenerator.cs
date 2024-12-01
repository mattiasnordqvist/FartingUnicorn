using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using System.Collections.Immutable;
using System.Text;

namespace MapperGenerator;

[Generator]
public class MapperGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "CreateMapperAttribute.g.cs",
            SourceText.From("""
                namespace DotNetThoughts.FartingUnicorn
                {
                    [System.AttributeUsage(System.AttributeTargets.Class)]
                    public class CreateMapperAttribute : System.Attribute
                    {
                    }
                }
                """, Encoding.UTF8)));

        IncrementalValuesProvider<ClassDeclarationSyntax> classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null);

        IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> compilationAndClasses
              = context.CompilationProvider.Combine(classDeclarations.Collect());
        context.RegisterSourceOutput(compilationAndClasses,
          static (spc, source) => Execute(source.Item1, source.Item2, spc));
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
           => node is ClassDeclarationSyntax m && m.AttributeLists.Count > 0;
    private static ClassDeclarationSyntax GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {

        var classDeclarationSyntaxNode = (ClassDeclarationSyntax)context.Node;

        foreach (AttributeListSyntax attributeListSyntax in classDeclarationSyntaxNode.AttributeLists)
        {
            foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                {
                    continue;
                }

                INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                string fullName = attributeContainingTypeSymbol.ToDisplayString();

                if (fullName == "DotNetThoughts.FartingUnicorn.CreateMapperAttribute")
                {
                    return classDeclarationSyntaxNode;
                }
            }
        }

        return null;
    }
    private static void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes, SourceProductionContext context)
    {
        if (classes.IsDefaultOrEmpty)
        {
            return;
        }

        // Get all referenced types that need mappers
        var referencedTypes = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);

        foreach (var classDeclaration in classes)
        {
            var semanticModel = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
            var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);
            if (classSymbol == null) continue;

            CollectReferencedTypes(classSymbol, referencedTypes, compilation);
        }

        // Generate mappers for all types
        var generatedMappers = new HashSet<string>();

        foreach (var classDeclaration in classes)
        {
            var model = BuildClassModel(classDeclaration, compilation);
            if (model != null)
            {
                GenerateMapperForClass(model, context, generatedMappers);
            }
        }

        foreach (var type in referencedTypes)
        {
            var syntaxRef = type.DeclaringSyntaxReferences.FirstOrDefault();
            if (syntaxRef == null) continue;

            var syntax = syntaxRef.GetSyntax() as ClassDeclarationSyntax;
            if (syntax == null) continue;

            var model = BuildClassModel(syntax, compilation);
            if (model != null)
            {
                GenerateMapperForClass(model, context, generatedMappers);
            }
        }
    }

    private static void CollectReferencedTypes(ITypeSymbol typeSymbol, HashSet<ITypeSymbol> referencedTypes, Compilation compilation)
    {
        foreach (var member in typeSymbol.GetMembers())
        {
            if (member is IPropertySymbol property)
            {
                var propertyType = property.Type;

                // If it's an array, get the element type
                if (propertyType is IArrayTypeSymbol arrayType)
                {
                    propertyType = arrayType.ElementType;
                }

                // If it's a named type (class) and not a built-in type
                if (propertyType is INamedTypeSymbol namedType &&
                    namedType.SpecialType == SpecialType.None &&
                    namedType.TypeKind == TypeKind.Class)
                {
                    if (referencedTypes.Add(propertyType))
                    {
                        // Recursively collect types from the referenced type
                        CollectReferencedTypes(propertyType, referencedTypes, compilation);
                    }
                }
            }
        }
    }

    private static void GenerateMapperForClass(ClassModel model, SourceProductionContext context, HashSet<string> generatedMappers)
    {
        var mapperName = $"{model.FullName}Mapper";
        if (!generatedMappers.Add(mapperName))
        {
            return; // Already generated
        }

        var sourceCode = GenerateMapper(model);
        context.AddSource($"{mapperName}.g.cs", sourceCode);
    }

    private static ClassModel? BuildClassModel(ClassDeclarationSyntax classDeclaration, Compilation compilation)
    {
        var semanticModel = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
        var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);

        if (classSymbol == null) return null;

        var properties = new List<PropertyModel>();

        foreach (var member in classDeclaration.Members)
        {
            if (member is PropertyDeclarationSyntax property)
            {
                var propertySymbol = semanticModel.GetDeclaredSymbol(property) as IPropertySymbol;
                if (propertySymbol == null) continue;

                var propertyType = propertySymbol.Type;
                var isReferenceType = propertyType.TypeKind == TypeKind.Class;
                var isNullable = propertyType.NullableAnnotation == NullableAnnotation.Annotated;
                var isOption = propertyType.FullTypeName().StartsWith("FartingUnicorn.Option<");

                var effectiveType = isOption
                    ? (isNullable && !isReferenceType
                        ? (((INamedTypeSymbol)propertyType).TypeArguments.First().FullTypeName())
                        : (((INamedTypeSymbol)propertyType).TypeArguments.First().FullTypeName()))
                    : (isNullable && !isReferenceType
                        ? (((INamedTypeSymbol)propertyType).TypeArguments.First().FullTypeName())
                        : propertyType.FullTypeName());
                var propertyModel = new PropertyModel
                {
                    Name = property.Identifier.Text,
                    TypeName = propertyType.ToDisplayString(),
                    IsArray = propertyType.TypeKind == TypeKind.Array,
                    IsReferenceType = isReferenceType,
                    IsNullable = isNullable,
                    IsOption = isOption,
                    EffectiveType = effectiveType
                };

                properties.Add(propertyModel);
            }
        }

        return new ClassModel
        {
            ClassName = classDeclaration.Identifier.Text,
            FullName = classSymbol.ToDisplayString(),
            Namespace = GetNamespace(classDeclaration),
            Properties = properties
        };
    }

    private static string GetNamespace(ClassDeclarationSyntax classDeclaration)
    {
        var namespaceName = string.Empty;
        var potentialNamespaceParent = classDeclaration.Parent;

        while (potentialNamespaceParent != null &&
               potentialNamespaceParent is not NamespaceDeclarationSyntax &&
               potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
        {
            potentialNamespaceParent = potentialNamespaceParent.Parent;
        }

        if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
        {
            namespaceName = namespaceParent.Name.ToString();
        }

        return namespaceName;
    }

    public class ClassModel
    {
        public string ClassName { get; set; }
        public string FullName { get; set; }
        public string Namespace { get; set; }
        public List<PropertyModel> Properties { get; set; }
        public bool HasCreateMapperAttribute { get; set; }

        public string[] ClassPath =>
                FullName.Substring(Namespace.Length + 1, FullName.Length - Namespace.Length - ClassName.Length - 1)
                    .Split('.').Select(x => x.Trim()).Where(x => x != string.Empty).ToArray();
    }

    public class PropertyModel
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
        public bool IsArray { get; set; }
        public bool IsReferenceType { get; set; }
        public bool IsNullable { get; set; }
        public bool IsOption { get; set; }
        public string EffectiveType { get; set; }
    }

    private static string GenerateMapper(ClassModel classModel)
    {
        var sb = new SourceBuilder();
        sb.AppendLine($@"// <auto-generated/>");
        sb.AppendLine("using DotNetThoughts.Results;");
        sb.AppendLine("using System.Text.Json;");
        sb.AppendLine("using static FartingUnicorn.MapperOptions;");
        sb.AppendLine();
        sb.AppendLine($"namespace {classModel.Namespace};");
        sb.AppendLine();


        sb.AppendLine($"// ClassName: {classModel.ClassName}");
        sb.AppendLine($"// FullName: {classModel.FullName}");
        sb.AppendLine($"// Namespace: {classModel.Namespace}");
        sb.AppendLine($"// Properties: {classModel.Properties.Count}");
        sb.AppendLine($"// HasCreateMapperAttribute: {classModel.HasCreateMapperAttribute}");
        sb.AppendLine($"// ClassPath: {string.Join(", ", classModel.ClassPath)}");
        sb.AppendLine();
        int i = 0;
        foreach (var p in classModel.Properties)
        {
            sb.AppendLine($"// Property {i}");
            sb.AppendLine($"// Name: {p.Name}");
            sb.AppendLine($"// TypeName: {p.TypeName}");
            sb.AppendLine($"// IsArray: {p.IsArray}");
            sb.AppendLine($"// IsReferenceType: {p.IsReferenceType}");
            sb.AppendLine($"// IsNullable: {p.IsNullable}");
            sb.AppendLine($"// IsOption: {p.IsOption}");
            sb.AppendLine($"// EffectiveType: {p.EffectiveType}");
            sb.AppendLine();
            i++;
        }
        sb.AppendLine();

        var stack = new Stack<IDisposable>();
        foreach (var c in classModel.ClassPath)
        {
            sb.AppendLine($"public partial class {c}");
            sb.AppendLine("{");
            stack.Push(sb.Indent());
        }

        // code

        sb.AppendLine($"public partial class {classModel.ClassName}");
        using (var _1 = sb.CodeBlock())
        {
            sb.AppendLine("// hello");
            sb.AppendLine($"public static Result<{classModel.ClassName}> MapFromJson(JsonElement jsonElement, MapperOptions mapperOptions = null, string[] path = null)");
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
                    sb.AppendLine($"return Result<{classModel.ClassName}>.Error(new ValueHasWrongTypeError(path, \"Object\", jsonElement.ValueKind.ToString()));");
                }

                sb.AppendLine($"var obj = new {classModel.ClassName}();");
                sb.AppendLine();
                sb.AppendLine("List<IError> errors = new();");

                foreach (var p in classModel.Properties)
                {
                    sb.AppendLine($"var is{p.Name}PropertyDefined = jsonElement.TryGetProperty(\"{p.Name}\", out var json{p.Name}Property);");
                    sb.AppendLine($"if (is{p.Name}PropertyDefined)");
                    using (var _3 = sb.CodeBlock())
                    {
                        sb.AppendLine($"// type = {p.TypeName}, isOption = {p.IsOption}, isNullable = {p.IsNullable}");
                        sb.AppendLine($"if (json{p.Name}Property.ValueKind == JsonValueKind.Null)");
                        using (var _4 = sb.CodeBlock())
                        {
                            if (p.IsOption)
                            {
                                sb.AppendLine($"obj.{p.Name} = new None<{p.EffectiveType}>();");
                            }
                            else
                            {
                                sb.AppendLine($"errors.Add(new RequiredValueMissingError([.. path, \"{p.Name}\"]));");
                            }
                        }
                        if (p.EffectiveType == "System.String")
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
                        else if (p.EffectiveType == "System.Boolean")
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
                        else if (p.EffectiveType == "System.Int32")
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
                        else
                        {
                            sb.AppendLine($"else if (mapperOptions.TryGetConverter(typeof({p.EffectiveType}), out IConverter customConverter))");
                            using (var _4 = sb.CodeBlock())
                            {
                                sb.AppendLine($"if (json{p.Name}Property.ValueKind != customConverter.ExpectedJsonValueKind)");
                                using (var _5 = sb.CodeBlock())
                                {
                                    sb.AppendLine($"errors.Add(new ValueHasWrongTypeError([.. path, \"{p.Name}\"], customConverter.ExpectedJsonValueKind.ToString(), json{p.Name}Property.ValueKind.ToString()));");
                                }
                                sb.AppendLine("else");
                                using (var _5 = sb.CodeBlock())
                                {
                                    sb.AppendLine($"var result = customConverter.Convert(typeof({p.EffectiveType}), json{p.Name}Property, mapperOptions, [.. path, \"{p.Name}\"]);");
                                    sb.AppendLine("if (result.Success)");
                                    using (var _6 = sb.CodeBlock())
                                    {
                                        if (p.IsOption)
                                            sb.AppendLine($"obj.{p.Name} = new Some<{p.EffectiveType}>(result.Map(x => ({p.EffectiveType})x).Value);");
                                        else
                                            sb.AppendLine($"obj.{p.Name} = result.Map(x => ({p.EffectiveType})x).Value;");
                                    }
                                    sb.AppendLine("else");
                                    using (var _6 = sb.CodeBlock())
                                    {
                                        sb.AppendLine($"errors.AddRange(result.Errors.Select(x => new MappingError([.. path, \"{p.Name}\"], x.Message)).ToArray());");
                                    }
                                }
                            }
                            if (p.IsReferenceType)
                            {
                                sb.AppendLine("else");
                                using (var _4 = sb.CodeBlock())
                                {
                                    sb.AppendLine($"if (json{p.Name}Property.ValueKind == JsonValueKind.Object)");
                                    using (var _5 = sb.CodeBlock())
                                    {
                                        sb.AppendLine($"var result = {p.EffectiveType}.MapFromJson(json{p.Name}Property, mapperOptions, [.. path, \"{p.Name}\"]);");
                                        sb.AppendLine("if (result.Success)");
                                        using (var _6 = sb.CodeBlock())
                                        {
                                            if (p.IsOption)
                                            {
                                                sb.AppendLine($"obj.{p.Name} = new Some<{p.EffectiveType}>(result.Value!);");

                                            }
                                            else
                                            {
                                                sb.AppendLine($"obj.{p.Name} = result.Value;");
                                            }
                                        }
                                        sb.AppendLine("else");
                                        using (var _6 = sb.CodeBlock())
                                        {
                                            sb.AppendLine($"errors.AddRange(result.Errors.ToArray());");
                                        }
                                    }
                                    sb.AppendLine("else");
                                    using (var _5 = sb.CodeBlock())
                                    {
                                        sb.AppendLine($"errors.Add(new ValueHasWrongTypeError([.. path, \"{p.Name}\"], \"Object\", json{p.Name}Property.ValueKind.ToString()));");
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
                    sb.AppendLine($"return Result<{classModel.ClassName}>.Error(errors);");
                }

                sb.AppendLine("if(false)/*check if is option*/");
                using (var _3 = sb.CodeBlock())
                {
                    // handle if option
                }
                sb.AppendLine("else");
                using (var _3 = sb.CodeBlock())
                {
                    sb.AppendLine($"return Result<{classModel.ClassName}>.Ok(obj);");
                }
                sb.AppendLine("throw new NotImplementedException();");

            }
        }

        // unindent
        while (stack.Count > 0)
        {
            stack.Pop().Dispose();
            sb.AppendLine("}");
        }

        return sb.ToString()!;
    }
}