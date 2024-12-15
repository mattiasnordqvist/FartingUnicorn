using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using System.Collections.Immutable;
using System.Diagnostics;
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

        IncrementalValuesProvider<RecordDeclarationSyntax> recordDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration2(s),
                transform: static (ctx, _) => GetSemanticTargetForGeneration2(ctx))
            .Where(static m => m is not null);


        IncrementalValueProvider<((Compilation, ImmutableArray<ClassDeclarationSyntax>), ImmutableArray<RecordDeclarationSyntax>)> compilationAndClasses
              = context.CompilationProvider.Combine(classDeclarations.Collect()).Combine(recordDeclarations.Collect());
        context.RegisterSourceOutput(compilationAndClasses,
          static (spc, source) => Execute(source.Item1.Item1, source.Item1.Item2, source.Item2, spc));
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
           => node is ClassDeclarationSyntax m && m.AttributeLists.Count > 0;

    private static bool IsSyntaxTargetForGeneration2(SyntaxNode node)
       => node is RecordDeclarationSyntax m && m.AttributeLists.Count > 0;
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
    private static RecordDeclarationSyntax GetSemanticTargetForGeneration2(GeneratorSyntaxContext context)
    {

        var classDeclarationSyntaxNode = (RecordDeclarationSyntax)context.Node;

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
    private static void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes, ImmutableArray<RecordDeclarationSyntax> records, SourceProductionContext context)
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

            if (syntaxRef.GetSyntax() is ClassDeclarationSyntax syntax)
            {
                var model = BuildClassModel(syntax, compilation);
                if (model != null)
                {
                    GenerateMapperForClass(model, context, generatedMappers);
                }
            }
        }

        foreach (var recordDeclaration in records)
        {
            var model = BuildClassModel(recordDeclaration, compilation);
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

                // If its an Options<T> type, get the T type
                if (propertyType.FullTypeName().StartsWith("FartingUnicorn.Option<"))
                {
                    var typeWithoutOptions = ((INamedTypeSymbol)propertyType).TypeArguments.First();
                    if (typeWithoutOptions is IArrayTypeSymbol typeWithoutOptionsArrayType)
                    {
                        typeWithoutOptions = typeWithoutOptionsArrayType.ElementType;
                    }
                    if (typeWithoutOptions is INamedTypeSymbol namedType2 &&
                       namedType2.SpecialType == SpecialType.None &&
                       namedType2.TypeKind == TypeKind.Class)
                    {
                        if (referencedTypes.Add(typeWithoutOptions))
                        {
                            // Recursively collect types from the referenced type
                            CollectReferencedTypes(typeWithoutOptions, referencedTypes, compilation);
                        }
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

                var completeType = propertySymbol.Type;
                var (rawType, isNullable, isOption) = CalculateType(completeType);

                var propertyModel = new PropertyModel
                {
                    Name = property.Identifier.Text,
                    CompleteType = completeType.ToDisplayString(),
                    IsArray = rawType.TypeKind == TypeKind.Array,
                    IsObject = rawType.TypeKind == TypeKind.Class && rawType.SpecialType == SpecialType.None,
                    IsNullable = isNullable,
                    IsOption = isOption,
                    RawType = rawType.FullTypeName(),
                };

                if (propertyModel.IsArray)
                {
                    var elementCompleteType = ((IArrayTypeSymbol)rawType).ElementType;
                    var (elementRawType, _, elementIsOption) = CalculateType(elementCompleteType);
                    var arrayModel = new ArrayElementModel
                    {
                        CompleteType = elementCompleteType.ToDisplayString(),
                        IsArray = elementRawType.TypeKind == TypeKind.Array,
                        IsObject = elementRawType.TypeKind == TypeKind.Class && elementRawType.SpecialType == SpecialType.None,
                        IsOption = elementIsOption,
                        RawType = elementRawType.FullTypeName(),
                    };
                    propertyModel.ArrayElementModel = arrayModel;
                }

                properties.Add(propertyModel);
            }
        }

        return new ClassModel
        {
            ClassName = classDeclaration.Identifier.Text,
            FullName = classSymbol.ToDisplayString(),
            Namespace = GetNamespace(classDeclaration),
            Properties = properties,
            IsRecord = false,
        };
    }

    private static ClassModel? BuildClassModel(RecordDeclarationSyntax recordDeclaration, Compilation compilation)
    {
        var semanticModel = compilation.GetSemanticModel(recordDeclaration.SyntaxTree);
        var recordSymbol = semanticModel.GetDeclaredSymbol(recordDeclaration);

        if (recordSymbol == null) return null;

        var primaryConstructor = recordSymbol.ConstructedFrom.InstanceConstructors
       .FirstOrDefault(constructor =>
           constructor.Parameters.Length == recordDeclaration.ParameterList?.Parameters.Count &&
           constructor.Parameters.Select(p => p.Name)
               .SequenceEqual(recordDeclaration.ParameterList.Parameters
                   .Select(p => p.Identifier.Text)));



        var properties = new List<PropertyModel>();
        foreach (var param in primaryConstructor.Parameters)
        {
            var completeType = param.Type;
            var (rawType, isNullable, isOption) = CalculateType(completeType);
            var propertyModel = new PropertyModel
            {
                Name = param.Name,
                CompleteType = completeType.ToDisplayString(),
                IsArray = rawType.TypeKind == TypeKind.Array,
                IsObject = rawType.TypeKind == TypeKind.Class && rawType.SpecialType == SpecialType.None,
                IsNullable = isNullable,
                IsOption = isOption,
                RawType = rawType.FullTypeName(),
            };
            if (propertyModel.IsArray)
            {
                var elementCompleteType = ((IArrayTypeSymbol)rawType).ElementType;
                var (elementRawType, _, elementIsOption) = CalculateType(elementCompleteType);
                var arrayModel = new ArrayElementModel
                {
                    CompleteType = elementCompleteType.ToDisplayString(),
                    IsArray = elementRawType.TypeKind == TypeKind.Array,
                    IsObject = elementRawType.TypeKind == TypeKind.Class && elementRawType.SpecialType == SpecialType.None,
                    IsOption = elementIsOption,
                    RawType = elementRawType.FullTypeName(),
                };
                propertyModel.ArrayElementModel = arrayModel;


            }
            properties.Add(propertyModel);

        }

        return new ClassModel
        {
            ClassName = recordDeclaration.Identifier.Text,
            FullName = recordSymbol.ToDisplayString(),
            Namespace = GetNamespace(recordDeclaration),
            Properties = properties,
            IsRecord = true,

        };
    }

    private static (ITypeSymbol rawType, bool isNullable, bool isOption) CalculateType(ITypeSymbol completeType)
    {

        var (nullabilityStripped, isNullable) = StripNullabilityFromType(completeType);
        var (optionalityStripped, isOption) = StripOptionalityFromType(nullabilityStripped);
        return (optionalityStripped, isNullable, isOption);
    }

    private static (ITypeSymbol, bool) StripNullabilityFromType(ITypeSymbol typeSymbol)
    {
        if (!typeSymbol.IsNullable())
            return (typeSymbol, false);
        if ((typeSymbol.IsNullableValueType() || typeSymbol.IsNullableEnumType()) && typeSymbol is INamedTypeSymbol namedTypeSymbol)
        {
            return (namedTypeSymbol.TypeArguments.First(), true);
        }
        if (typeSymbol.IsNullable() && typeSymbol.FullTypeName().StartsWith("FartingUnicorn.Option<") && typeSymbol is INamedTypeSymbol)
        {
            return (typeSymbol, true);
        }
        // nullable reference types are just annotated, so its ok to just return here
        return (typeSymbol, true);
    }

    private static (ITypeSymbol, bool) StripOptionalityFromType(ITypeSymbol typeSymbol)
    {
        if (!typeSymbol.FullTypeName().StartsWith("FartingUnicorn.Option<"))
            return (typeSymbol, false);
        return (((INamedTypeSymbol)typeSymbol).TypeArguments.First(), true);
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

    private static string GetNamespace(RecordDeclarationSyntax classDeclaration)
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

        public bool IsRecord { get; set; }
    }
    public class ArrayElementModel
    {
        public string CompleteType { get; set; }
        public bool IsArray { get; set; }
        public bool IsObject { get; set; }
        public bool IsOption { get; set; }
        public string RawType { get; set; }
    }
    public class PropertyModel
    {
        public string Name { get; set; }
        public string CompleteType { get; set; }
        public bool IsArray { get; set; }
        public bool IsObject { get; set; }
        public bool IsNullable { get; set; }
        public bool IsOption { get; set; }
        public string RawType { get; set; }
        public ArrayElementModel ArrayElementModel { get; set; }
    }

    private static string GenerateMapper(ClassModel classModel)
    {
        var sb = new SourceBuilder();
        sb.AppendLine($@"// <auto-generated/>");
        sb.AppendLine("using DotNetThoughts.Results;");
        sb.AppendLine("using System.Text.Json;");
        sb.AppendLine("using FartingUnicorn;");
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
            sb.AppendLine($"// CompleteType: {p.CompleteType}");
            sb.AppendLine($"// IsArray: {p.IsArray}");
            sb.AppendLine($"// IsObject: {p.IsObject}");
            sb.AppendLine($"// IsNullable: {p.IsNullable}");
            sb.AppendLine($"// IsOption: {p.IsOption}");
            sb.AppendLine($"// RawType: {p.RawType}");

            if (p.IsArray)
            {
                sb.AppendLine($"// ArrayElemCompleteType: {p.ArrayElementModel.CompleteType}");
                sb.AppendLine($"// IsArrayElemArray: {p.ArrayElementModel.IsArray}");
                sb.AppendLine($"// IsArrayElemObject: {p.ArrayElementModel.IsObject}");
                sb.AppendLine($"// IsArrayElemOption: {p.ArrayElementModel.IsOption}");
                sb.AppendLine($"// ArrayElemRawType: {p.ArrayElementModel.RawType}");
            }

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

        sb.AppendLine($"public partial {(classModel.IsRecord ? "record" : "class")} {classModel.ClassName}");
        using (var _1 = sb.CodeBlock())
        {
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
                        sb.AppendLine($"if (json{p.Name}Property.ValueKind == JsonValueKind.Null)");
                        using (var _4 = sb.CodeBlock())
                        {
                            if (p.IsOption)
                            {
                                sb.AppendLine($"obj.{p.Name} = new None<{p.RawType}>();");
                            }
                            else
                            {
                                sb.AppendLine($"errors.Add(new RequiredValueMissingError([.. path, \"{p.Name}\"]));");
                            }
                        }
                        if (p.RawType == "System.String")
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
                        else if (p.RawType == "System.Boolean")
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
                        else if (p.RawType == "System.Int32")
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
                        else if (p.IsArray)
                        {
                            sb.AppendLine($"else if (json{p.Name}Property.ValueKind == JsonValueKind.Array)");
                            using (var _4 = sb.CodeBlock())
                            {
                                sb.AppendLine($"var array = new {p.ArrayElementModel.CompleteType}[json{p.Name}Property.GetArrayLength()];");

                                sb.AppendLine($"for(int i = 0; i < json{p.Name}Property.GetArrayLength(); i++)");
                                using (var _5 = sb.CodeBlock())
                                {
                                    if (p.ArrayElementModel.RawType == "System.String")
                                    {
                                        sb.AppendLine($"if(json{p.Name}Property[i].ValueKind != JsonValueKind.String)");
                                        using (var _6 = sb.CodeBlock())
                                        {
                                            sb.AppendLine($"errors.Add(new ValueHasWrongTypeError([.. path, \"{p.Name}\", i.ToString()], \"String\", json{p.Name}Property[i].ValueKind.ToString()));");
                                        }
                                        sb.AppendLine($"else");
                                        using (var _6 = sb.CodeBlock())
                                        {
                                            sb.AppendLine($"array.SetValue(json{p.Name}Property[i].GetString(), i);");
                                        }
                                    }
                                    else if (p.ArrayElementModel.RawType == "System.Boolean")
                                    {
                                        sb.AppendLine($"if(json{p.Name}Property[i].ValueKind != JsonValueKind.True && json{p.Name}Property[i].ValueKind != JsonValueKind.False)");
                                        using (var _6 = sb.CodeBlock())
                                        {
                                            sb.AppendLine($"errors.Add(new ValueHasWrongTypeError([.. path, \"{p.Name}\", i.ToString()], \"Boolean\", json{p.Name}Property[i].ValueKind.ToString()));");
                                        }
                                        sb.AppendLine($"else");
                                        using (var _6 = sb.CodeBlock())
                                        {
                                            sb.AppendLine($"array.SetValue(json{p.Name}Property[i].GetBoolean(), i);");
                                        }
                                    }
                                    else if (p.ArrayElementModel.RawType == "System.Int32")
                                    {
                                        sb.AppendLine($"if(json{p.Name}Property[i].ValueKind != JsonValueKind.Number)");
                                        using (var _6 = sb.CodeBlock())
                                        {
                                            sb.AppendLine($"errors.Add(new ValueHasWrongTypeError([.. path, \"{p.Name}\", i.ToString()], \"Number\", json{p.Name}Property[i].ValueKind.ToString()));");
                                        }
                                        sb.AppendLine($"else");
                                        using (var _6 = sb.CodeBlock())
                                        {
                                            sb.AppendLine($"array.SetValue(json{p.Name}Property[i].GetInt32(), i);");
                                        }
                                    }
                                    else if (p.ArrayElementModel.IsObject)
                                    {
                                        sb.AppendLine($"var result = {p.ArrayElementModel.RawType}.MapFromJson(json{p.Name}Property[i], mapperOptions, [.. path, \"{p.Name}\", i.ToString()]);");
                                        sb.AppendLine("if (result.Success)");
                                        using (var _6 = sb.CodeBlock())
                                        {
                                            sb.AppendLine($"array.SetValue(result.Value, i);");
                                        }
                                        sb.AppendLine("else");
                                        using (var _6 = sb.CodeBlock())
                                        {
                                            sb.AppendLine($"errors.AddRange(result.Errors.ToArray());");
                                        }
                                    }
                                }
                                if (p.IsOption)
                                {
                                    sb.AppendLine($"obj.{p.Name} = new Some<{p.ArrayElementModel.CompleteType}[]>(array);");
                                }
                                else
                                {
                                    sb.AppendLine($"obj.{p.Name} = array;");
                                }
                            }
                            sb.AppendLine("else");
                            using (var _4 = sb.CodeBlock())
                            {
                                sb.AppendLine($"errors.Add(new ValueHasWrongTypeError([.. path, \"{p.Name}\"], \"Array\", json{p.Name}Property.ValueKind.ToString()));");
                            }
                        }
                        else
                        {
                            sb.AppendLine($"else if (mapperOptions.TryGetConverter(typeof({p.RawType}), out IConverter customConverter))");
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
                                    sb.AppendLine($"var result = customConverter.Convert(typeof({p.RawType}), json{p.Name}Property, mapperOptions, [.. path, \"{p.Name}\"]);");
                                    sb.AppendLine("if (result.Success)");
                                    using (var _6 = sb.CodeBlock())
                                    {
                                        if (p.IsOption)
                                            sb.AppendLine($"obj.{p.Name} = new Some<{p.RawType}>(result.Map(x => ({p.RawType})x).Value);");
                                        else
                                            sb.AppendLine($"obj.{p.Name} = result.Map(x => ({p.RawType})x).Value;");
                                    }
                                    sb.AppendLine("else");
                                    using (var _6 = sb.CodeBlock())
                                    {
                                        sb.AppendLine($"errors.AddRange(result.Errors.Select(x => new MappingError([.. path, \"{p.Name}\"], x.Message)).ToArray());");
                                    }
                                }
                            }
                            if (p.IsObject)
                            {
                                sb.AppendLine("else");
                                using (var _4 = sb.CodeBlock())
                                {
                                    sb.AppendLine($"if (json{p.Name}Property.ValueKind == JsonValueKind.Object)");
                                    using (var _5 = sb.CodeBlock())
                                    {
                                        sb.AppendLine($"var result = {p.RawType}.MapFromJson(json{p.Name}Property, mapperOptions, [.. path, \"{p.Name}\"]);");
                                        sb.AppendLine("if (result.Success)");
                                        using (var _6 = sb.CodeBlock())
                                        {
                                            if (p.IsOption)
                                            {
                                                sb.AppendLine($"obj.{p.Name} = new Some<{p.RawType}>(result.Value!);");

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