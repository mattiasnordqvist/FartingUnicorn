using DotNetThoughts.Results;

using Namotion.Reflection;

using System.Reflection;
using System.Text.Json;

namespace FartingUnicorn;

public class Mapper
{
    public record RequiredPropertyMissingError(string propertyName) : ErrorBase($"{propertyName} is required");
    public record RequiredValueMissingError(string propertyName) : ErrorBase($"{propertyName} must have a value");
    public record ValueHasWrongTypeError(string propertyName, string expectedType, string actualType) : ErrorBase($"Value of {propertyName} has the wrong type. Expected {expectedType}, got {actualType}");
    public static Result<T> Map<T>(JsonElement json, string[] path = null)
        where T : new()
    {
        if (path is null)
        {
            path = Array.Empty<string>();
        }
        // Rewrite this with source generator to avoid type generics and reflection
        return MapObject<T>(json, path);
    }

    public static MethodInfo MapObjectMethod { get; } = typeof(Mapper).GetMethod(nameof(MapObject), BindingFlags.Public | BindingFlags.Static) ?? throw new Exception("Can't find MapObject method");

    public static Result<T> MapObject<T>(JsonElement json, string[] path)
        where T : new()
    {

        Result<Unit> validationResult = UnitResult.Ok;
        var obj = new T();

        foreach (var contextualProperty in typeof(T).GetContextualProperties())
        {
            var property = contextualProperty.PropertyInfo;
            var isPropertyDefined = json.TryGetProperty(property.Name, out var jsonProperty);
            if (isPropertyDefined)
            {
                if (jsonProperty.ValueKind == JsonValueKind.Null)
                {
                    if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Option<>))
                    {
                        var noneType = typeof(None<>).MakeGenericType(property.PropertyType.GetGenericArguments()[0]);
                        property.SetValue(obj, Activator.CreateInstance(noneType));
                    }
                    else
                    {
                        validationResult = validationResult.Or(Result<Unit>.Error(new RequiredValueMissingError(property.Name)));
                    }
                }
                else
                {
                    if (property.PropertyType == typeof(int)
                        || property.PropertyType == typeof(Option<int>)
                        || property.PropertyType == typeof(int?))
                    {
                        if (jsonProperty.ValueKind != JsonValueKind.Number)
                        {
                            validationResult = validationResult.Or(Result<Unit>.Error(new ValueHasWrongTypeError(property.Name, "Number", jsonProperty.ValueKind.ToString())));
                        }
                        else
                        {
                            if (property.PropertyType == typeof(Option<int>))
                            {
                                property.SetValue(obj, new Some<int>(jsonProperty.GetInt32()));
                            }
                            else
                            {
                                property.SetValue(obj, jsonProperty.GetInt32());
                            }
                        }
                    }
                    else if (property.PropertyType == typeof(bool)
                        || property.PropertyType == typeof(Option<bool>)
                        || property.PropertyType == typeof(bool?))
                    {
                        if (jsonProperty.ValueKind != JsonValueKind.True && jsonProperty.ValueKind != JsonValueKind.False)
                        {
                            validationResult = validationResult.Or(Result<Unit>.Error(new ValueHasWrongTypeError(property.Name, "Boolean", jsonProperty.ValueKind.ToString())));
                        }
                        else
                        {
                            if (property.PropertyType == typeof(Option<bool>))
                            {
                                property.SetValue(obj, new Some<bool>(jsonProperty.GetBoolean()));
                            }
                            else
                            {
                                property.SetValue(obj, jsonProperty.GetBoolean());
                            }
                        }
                    }
                    else if (property.PropertyType == typeof(string)
                        || property.PropertyType == typeof(Option<string>))
                    {
                        if (jsonProperty.ValueKind != JsonValueKind.String)
                        {
                            validationResult = validationResult.Or(Result<Unit>.Error(new ValueHasWrongTypeError(property.Name, "String", jsonProperty.ValueKind.ToString())));
                        }
                        else
                        {
                            if (property.PropertyType == typeof(Option<string>))
                            {
                                property.SetValue(obj, new Some<string>(jsonProperty.GetString()));
                            }
                            else
                            {
                                property.SetValue(obj, jsonProperty.GetString());
                            }
                        }
                    }
                    else if (property.PropertyType.IsArray
                        || (property.PropertyType.IsGenericType
                            && property.PropertyType.GetGenericTypeDefinition() == typeof(Option<>)
                            && property.PropertyType.GetGenericArguments()[0].IsArray))
                    {
                        if (jsonProperty.ValueKind != JsonValueKind.Array)
                        {
                            validationResult = validationResult.Or(Result<Unit>.Error(new ValueHasWrongTypeError(property.Name, "Array", jsonProperty.ValueKind.ToString())));
                        }
                        else
                        {
                            var arrayPath = path.Append(property.Name).ToArray();

                            if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Option<>))
                            {
                                var elementType = property.PropertyType.GetGenericArguments()[0].GetElementType();
                                
                                var genericMapMethod = MapObjectMethod.MakeGenericMethod(elementType);
                                var array = Array.CreateInstance(elementType, jsonProperty.GetArrayLength());
                                var errors = new List<IError>();
                                for (int i = 0; i < jsonProperty.GetArrayLength(); i++)
                                {
                                    var arrayElementPath = arrayPath.Append(i.ToString()).ToArray();
                                    var result = genericMapMethod.Invoke(null, [jsonProperty[i], arrayElementPath]);
                                    var resultSuccessProperty = result.GetType().GetProperty("Success");

                                    if ((bool)resultSuccessProperty.GetValue(result))
                                    {

                                        var valueProperty = result.GetType().GetProperty("Value");
                                        array.SetValue(valueProperty.GetValue(result), i);
                                    }
                                    else
                                    {
                                        var errorProperty = result.GetType().GetProperty("Errors");
                                        var elementErrors = (IReadOnlyList<IError>)errorProperty.GetValue(result);
                                        errors.AddRange(elementErrors);
                                    }
                                }
                                if (errors.Any())
                                {
                                    var newErrorsResult = UnitResult.Ok;
                                    foreach (var error in errors)
                                    {
                                        newErrorsResult = newErrorsResult.Or(Result<Unit>.Error(error));
                                    }
                                    validationResult = validationResult.Or(newErrorsResult);
                                }
                                else
                                {
                                    var someType = typeof(Some<>).MakeGenericType(property.PropertyType.GetGenericArguments()[0]);
                                    var someInstance = Activator.CreateInstance(someType, array);
                                    property.SetValue(obj, someInstance);
                                }
                            }
                            else
                            {

                                var elementType = property.PropertyType.GetElementType();
                                var genericMapMethod = MapObjectMethod.MakeGenericMethod(elementType);
                                var array = Array.CreateInstance(elementType, jsonProperty.GetArrayLength());
                                var errors = new List<IError>();
                                for (int i = 0; i < jsonProperty.GetArrayLength(); i++)
                                {
                                    var arrayElementPath = arrayPath.Append(i.ToString()).ToArray();
                                    var result = genericMapMethod.Invoke(null, [jsonProperty[i], arrayElementPath]);
                                    var resultSuccessProperty = result.GetType().GetProperty("Success");

                                    if ((bool)resultSuccessProperty.GetValue(result))
                                    {

                                        var valueProperty = result.GetType().GetProperty("Value");
                                        array.SetValue(valueProperty.GetValue(result), i);
                                    }
                                    else
                                    {
                                        var errorProperty = result.GetType().GetProperty("Errors");
                                        var elementErrors = (IReadOnlyList<IError>)errorProperty.GetValue(result);
                                        errors.AddRange(elementErrors);
                                    }
                                }
                                if (errors.Any())
                                {
                                    var newErrorsResult = UnitResult.Ok;
                                    foreach (var error in errors)
                                    {
                                        newErrorsResult = newErrorsResult.Or(Result<Unit>.Error(error));
                                    }
                                    validationResult = validationResult.Or(newErrorsResult);
                                }
                                else
                                {
                                    property.SetValue(obj, array);
                                }
                            }
                        }
                    }
                    else /*object!*/
                    {
                        if (jsonProperty.ValueKind != JsonValueKind.Object)
                        {
                            validationResult = validationResult.Or(Result<Unit>.Error(new ValueHasWrongTypeError(property.Name, "Object", jsonProperty.ValueKind.ToString())));
                        }
                        else
                        {
                            var newPath = path.Append(property.Name).ToArray();

                            if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Option<>))
                            {
                                var genericMapMethod = MapObjectMethod.MakeGenericMethod(property.PropertyType.GetGenericArguments()[0]);
                                var result = genericMapMethod.Invoke(null, [jsonProperty, newPath]);
                                var resultSuccessProperty = result.GetType().GetProperty("Success");

                                if ((bool)resultSuccessProperty.GetValue(result))
                                {
                                    var valueProperty = result.GetType().GetProperty("Value");
                                    var getValue = valueProperty.GetValue(result);
                                    var someType = typeof(Some<>).MakeGenericType(property.PropertyType.GetGenericArguments()[0]);
                                    var someInstance = Activator.CreateInstance(someType, getValue);
                                    property.SetValue(obj, someInstance);
                                }
                                else
                                {
                                    var errorProperty = result.GetType().GetProperty("Errors");
                                    var errors = (IReadOnlyList<IError>)errorProperty.GetValue(result);
                                    var newErrorsResult = UnitResult.Ok;
                                    foreach (var error in errors)
                                    {
                                        newErrorsResult = newErrorsResult.Or(Result<Unit>.Error(error));
                                    }
                                    validationResult = validationResult.Or(newErrorsResult);
                                }
                            }
                            else
                            {
                                var genericMapMethod = MapObjectMethod.MakeGenericMethod(property.PropertyType);
                                var result = genericMapMethod.Invoke(null, [jsonProperty, newPath]);
                                var resultSuccessProperty = result.GetType().GetProperty("Success");

                                if ((bool)resultSuccessProperty.GetValue(result))
                                {
                                    var valueProperty = result.GetType().GetProperty("Value");
                                    property.SetValue(obj, valueProperty.GetValue(result));
                                }
                                else
                                {
                                    var errorProperty = result.GetType().GetProperty("Errors");
                                    var errors = (IReadOnlyList<IError>)errorProperty.GetValue(result);
                                    var newErrorsResult = UnitResult.Ok;
                                    foreach (var error in errors)
                                    {
                                        newErrorsResult = newErrorsResult.Or(Result<Unit>.Error(error));
                                    }
                                    validationResult = validationResult.Or(newErrorsResult);
                                }
                            }
                        }
                    }
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
                    validationResult = validationResult.Or(Result<Unit>.Error(new RequiredPropertyMissingError(string.Join(".", path.Append(property.Name)))));
                }
            }
        }

        return validationResult.Map(() => obj);
    }
}
