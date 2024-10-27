using DotNetThoughts.Results;

using Namotion.Reflection;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

using static FartingUnicorn.MapperOptions;

namespace FartingUnicorn;
public class MapperOptions
{
    public interface IConverter
    {
        bool CanConvert(Type type);

        JsonValueKind ExpectedJsonValueKind { get; }

        Result<object> Convert(JsonElement jsonElement, MapperOptions mapperOptions, string[] path);
    }

    public List<IConverter> _converters = [];

    public void AddConverter(IConverter converter)
    {
        _converters.Add(converter);
    }

    public bool TryGetConverter(Type type, [NotNullWhen(true)] out IConverter? converter)
    {
        converter = _converters.FirstOrDefault(x => x.CanConvert(type));
        return converter != null;
    }
}
public class Mapper
{
    public abstract record FartingUnicornErrorBase(string[] Path, string Message) : ErrorBase(Message);
    public record RequiredPropertyMissingError(string[] path) : FartingUnicornErrorBase(path, $"{string.Join(".", path)} is required");
    public record RequiredValueMissingError(string[] path) : FartingUnicornErrorBase(path, $"{string.Join(".", path)} must have a value");
    public record ValueHasWrongTypeError(string[] path, string expectedType, string actualType) : FartingUnicornErrorBase(path, $"Value of {string.Join(".", path)} has the wrong type. Expected {expectedType}, got {actualType}");
    public static Result<T> Map<T>(JsonElement json, MapperOptions mapperOptions = null, string[] path = null)
    {
        return MapElement(typeof(T), json, mapperOptions, path).Map(x => (T)x);
    }

    private static (bool isOption, Type type) D(Type type)
    {
        var isOption = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Option<>);
        var innerType = isOption ? type.GetGenericArguments()[0] : type;
        return (isOption, innerType);
    }
    public static Result<object> MapElement<T>(JsonElement jsonElement, MapperOptions mapperOptions = null, string[] path = null)
    {
        return MapElement(typeof(T), jsonElement, mapperOptions, path);
    }
    public static Result<object> MapElement(Type t, JsonElement jsonElement, MapperOptions mapperOptions = null, string[] path = null)
    {
        if (mapperOptions is null)
        {
            mapperOptions = new MapperOptions();
        }
        if (path is null)
        {
            path = ["$"];
        }
        var (isOption, type) = D(t);
        if (jsonElement.ValueKind == JsonValueKind.Null)
        {
            if (isOption)
            {
                var noneType = typeof(None<>).MakeGenericType(type);
                return Result<object>.Ok(Activator.CreateInstance(noneType));
            }
            else
            {
                return Result<object>.Error(new RequiredValueMissingError(path));
            }
        }

        if (type == typeof(string))
        {
            if (jsonElement.ValueKind != JsonValueKind.String)
            {
                return Result<object>.Error(new ValueHasWrongTypeError(path, "String", jsonElement.ValueKind.ToString()));
            }
            else
            {
                if (isOption)
                {
                    return Result<object>.Ok(new Some<string>(jsonElement.GetString()));
                }
                else
                {
                    return Result<object>.Ok(jsonElement.GetString());
                }
            }
        }

        if (type == typeof(int) || type == typeof(int?))
        {
            if (jsonElement.ValueKind != JsonValueKind.Number)
            {
                return Result<object>.Error(new ValueHasWrongTypeError(path, "Number", jsonElement.ValueKind.ToString()));
            }
            else
            {
                if (isOption)
                {
                    return Result<object>.Ok(new Some<int>(jsonElement.GetInt32()));
                }
                else
                {
                    return Result<object>.Ok(jsonElement.GetInt32());
                }
            }
        }

        if (type == typeof(bool) || type == typeof(bool?))
        {
            if (jsonElement.ValueKind != JsonValueKind.True && jsonElement.ValueKind != JsonValueKind.False)
            {
                return Result<object>.Error(new ValueHasWrongTypeError(path, "Boolean", jsonElement.ValueKind.ToString()));
            }
            else
            {
                if (isOption)
                {
                    return Result<object>.Ok(new Some<bool>(jsonElement.GetBoolean()));
                }
                else
                {
                    return Result<object>.Ok(jsonElement.GetBoolean());
                }
            }
        }

        if (type.IsArray)
        {
            if (jsonElement.ValueKind != JsonValueKind.Array)
            {
                return Result<Unit>.Error(new ValueHasWrongTypeError(path, "Array", jsonElement.ValueKind.ToString()));
            }
            var elementType = type.GetElementType()!;

            var array = Array.CreateInstance(elementType, jsonElement.GetArrayLength());
            Result<Unit> compositeResult = UnitResult.Ok;

            for (int i = 0; i < jsonElement.GetArrayLength(); i++)
            {
                var mapResult = MapElement(elementType, jsonElement[i], mapperOptions, [.. path, i.ToString()]);
                if (mapResult.Success)
                {
                    array.SetValue(mapResult.Value, i);
                }
                else
                {
                    compositeResult = compositeResult.Or(mapResult);
                }
            }

            if (!compositeResult.Success)
            {
                return Result<object>.Error(compositeResult.Errors);
            }

            if (isOption)
            {
                var someType = typeof(Some<>).MakeGenericType(type);
                var someInstance = Activator.CreateInstance(someType, array);
                return Result<object>.Ok(someInstance);
            }
            else
            {

                return Result<object>.Ok(array);
            }
        }

        if (mapperOptions.TryGetConverter(type, out IConverter customConverter))
        {
            if (jsonElement.ValueKind != customConverter.ExpectedJsonValueKind)
            {
                return Result<object>.Error(new ValueHasWrongTypeError(path, customConverter.ExpectedJsonValueKind.ToString(), jsonElement.ValueKind.ToString()));
            }

            return customConverter.Convert(jsonElement, mapperOptions, path);
        }

        /*object*/
        {
            if (jsonElement.ValueKind != JsonValueKind.Object)
            {
                return Result<object>.Error(new ValueHasWrongTypeError(path, "Object", jsonElement.ValueKind.ToString()));
            }

            var obj = Activator.CreateInstance(type)!;

            Result<Unit> compositeResult = UnitResult.Ok;
            foreach (var contextualProperty in type.GetContextualProperties())
            {
                var property = contextualProperty.PropertyInfo;
                var isPropertyDefined = jsonElement.TryGetProperty(property.Name, out var jsonProperty);
                if (isPropertyDefined)
                {
                    var mapResult = MapElement(property.PropertyType, jsonProperty, mapperOptions, [.. path, property.Name]);
                    if (mapResult.Success)
                    {
                        property.SetValue(obj, mapResult.Value);
                    }
                    else
                    {
                        compositeResult = compositeResult.Or(mapResult);
                    }
                }
                else
                {
                    if (contextualProperty.Nullability == Nullability.Nullable)
                    {
                        property.SetValue(obj, null);
                    }
                    else
                    {
                        compositeResult = compositeResult.Or(Result<Unit>.Error(new RequiredPropertyMissingError([.. path, property.Name])));
                    }
                }
            }

            if (!compositeResult.Success)
            {
                return Result<object>.Error(compositeResult.Errors);
            }


            if (isOption)
            {
                var someType = typeof(Some<>).MakeGenericType(type);
                var someInstance = Activator.CreateInstance(someType, obj);
                return Result<object>.Ok(someInstance);
            }
            else
            {

                return Result<object>.Ok(obj);
            }

        }

        throw new NotImplementedException();

    }
}
