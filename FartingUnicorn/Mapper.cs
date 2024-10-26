﻿using DotNetThoughts.Results;

using Namotion.Reflection;

using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection;
using System.Text.Json;

using static System.Runtime.InteropServices.JavaScript.JSType;


namespace FartingUnicorn;

public class Mapper
{
    public record RequiredPropertyMissingError(string[] path) : ErrorBase($"{string.Join(".", path)} is required");
    public record RequiredValueMissingError(string[] path) : ErrorBase($"{string.Join(".", path)} must have a value");
    public record ValueHasWrongTypeError(string[] path, string expectedType, string actualType) : ErrorBase($"Value of {string.Join(".", path)} has the wrong type. Expected {expectedType}, got {actualType}");
    public static Result<T> Map<T>(JsonElement json, string[] path = null)
        where T : new()
    {
        if (path is null)
        {
            path = Array.Empty<string>();
        }
        // Rewrite this with source generator to avoid type generics and reflection
        return MapObject(typeof(T), json, path).Map(x => (T)x);
    }

    public static Result<object> MapObject(Type type, JsonElement json, string[] path)
    {
        Result<Unit> validationResult = UnitResult.Ok;
        var obj = Activator.CreateInstance(type)!;

        foreach (var contextualProperty in type.GetContextualProperties())
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
                        validationResult = validationResult.Or(Result<Unit>.Error(new RequiredValueMissingError([property.Name])));
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
                            validationResult = validationResult.Or(Result<Unit>.Error(new ValueHasWrongTypeError([property.Name], "Number", jsonProperty.ValueKind.ToString())));
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
                            validationResult = validationResult.Or(Result<Unit>.Error(new ValueHasWrongTypeError([property.Name], "Boolean", jsonProperty.ValueKind.ToString())));
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
                            validationResult = validationResult.Or(Result<Unit>.Error(new ValueHasWrongTypeError([property.Name], "String", jsonProperty.ValueKind.ToString())));
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
                            validationResult = validationResult.Or(Result<Unit>.Error(new ValueHasWrongTypeError([property.Name], "Array", jsonProperty.ValueKind.ToString())));
                        }
                        else
                        {
                            var arrayPath = path.Append(property.Name).ToArray();
                            var isOption = property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Option<>);
                            var elementType = isOption
                                ? property.PropertyType.GetGenericArguments()[0].GetElementType()
                                : property.PropertyType.GetElementType();

                            var array = Array.CreateInstance(elementType, jsonProperty.GetArrayLength());
                            var errors = UnitResult.Ok;
                            for (int i = 0; i < jsonProperty.GetArrayLength(); i++)
                            {
                                var arrayElementPath = arrayPath.Append(i.ToString()).ToArray();

                                var result = MapObject(elementType, jsonProperty[i], arrayElementPath);

                                MapResultToArrayIndexAndValidationResult(result, array, i, ref errors);
                            }
                            if (!errors.Success)
                            {
                                var newErrorsResult = UnitResult.Ok;
                                foreach (var error in errors.Errors)
                                {
                                    newErrorsResult = newErrorsResult.Or(Result<Unit>.Error(error));
                                }
                                validationResult = validationResult.Or(newErrorsResult);
                            }
                            else
                            {
                                if (isOption)
                                {
                                    var someType = typeof(Some<>).MakeGenericType(property.PropertyType.GetGenericArguments()[0]);
                                    var someInstance = Activator.CreateInstance(someType, array);
                                    property.SetValue(obj, someInstance);
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
                            validationResult = validationResult.Or(Result<Unit>.Error(new ValueHasWrongTypeError([property.Name], "Object", jsonProperty.ValueKind.ToString())));
                        }
                        else
                        {
                            var newPath = path.Append(property.Name).ToArray();

                            var propertyType = property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Option<>)
                                ? property.PropertyType.GetGenericArguments()[0]
                                : property.PropertyType;

                            var result = MapObject(propertyType, jsonProperty, newPath);
                            MapResultToPropertyAndValidationResult(result, obj, property, ref validationResult);
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
                    validationResult = validationResult.Or(Result<Unit>.Error(new RequiredPropertyMissingError([.. path, property.Name])));
                }
            }
        }

        return validationResult.Map(() => obj);
    }

    private static void MapResultToArrayIndexAndValidationResult(Result<object> result, Array array, int i, ref Result<Unit> validationResult)
    {
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
            var newErrorsResult = UnitResult.Ok;
            foreach (var error in elementErrors)
            {
                newErrorsResult = newErrorsResult.Or(Result<Unit>.Error(error));
            }
            validationResult = validationResult.Or(newErrorsResult);
        }
    }

    private static void MapResultToPropertyAndValidationResult(Result<object> result, object obj, PropertyInfo property, ref Result<Unit> validationResult)
    {
        var resultSuccessProperty = result.GetType().GetProperty("Success");
        var isOption = (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Option<>));

        if ((bool)resultSuccessProperty.GetValue(result))
        {
            var valueProperty = result.GetType().GetProperty("Value");
            var getValue = valueProperty.GetValue(result);

            if (!isOption)
            {
                property.SetValue(obj, getValue);
            }
            else
            {
                var someType = typeof(Some<>).MakeGenericType(property.PropertyType.GetGenericArguments()[0]);
                var someInstance = Activator.CreateInstance(someType, getValue);
                property.SetValue(obj, someInstance);
            }
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

    private static bool IsOption(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Option<>);
    }

    private static (bool isOption, Type type) D(Type type)
    {
        var isOption = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Option<>);
        var innerType = isOption ? type.GetGenericArguments()[0] : type;
        return (isOption, innerType);
    }
    public static Result<object> MapElement<T>(JsonElement jsonElement, string[] path = null)
    {
        return MapElement(typeof(T), jsonElement, path);
    }
    public static Result<object> MapElement(Type t, JsonElement jsonElement, string[] path = null)
    {
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

        if (type == typeof(int))
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

        if (type == typeof(bool))
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
                var mapResult = MapElement(elementType, jsonElement[i], [.. path, i.ToString()]);
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
                    var mapResult = MapElement(property.PropertyType, jsonProperty, [.. path, property.Name]);
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
