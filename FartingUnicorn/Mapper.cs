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
    public static Result<T> Map<T>(JsonElement json)
        where T : new()
    {
        // Rewrite this with source generator to avoid type generics and reflection

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
                    else /*object!*/
                    {
                        if(jsonProperty.ValueKind != JsonValueKind.Object)
                        {
                            validationResult = validationResult.Or(Result<Unit>.Error(new ValueHasWrongTypeError(property.Name, "Object", jsonProperty.ValueKind.ToString())));
                        }
                        else
                        {
                            var mapMethod = typeof(Mapper).GetMethod("Map", BindingFlags.Public | BindingFlags.Static);
                            var genericMapMethod = mapMethod.MakeGenericMethod(property.PropertyType);
                            var result = genericMapMethod.Invoke(null, [jsonProperty]);
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
            else
            {
                if (contextualProperty.Nullability == Nullability.Nullable)
                {
                    property.SetValue(obj, null);
                }
                else
                {
                    validationResult = validationResult.Or(Result<Unit>.Error(new RequiredPropertyMissingError(property.Name)));
                }
            }
        }

        return validationResult.Map(() => obj);
        //// required property in all senses. It must be present in the JSON and must have a value (not null in json)
        //if (!json.TryGetProperty("name", out var nameProperty))
        //{
        //    // This handles the case when the property is completely missing
        //    result = result.Or(Result<Unit>.Error(new RequiredPropertyMissingError("name")));
        //}
        //else
        //{
        //    if (nameProperty.ValueKind == JsonValueKind.Null)
        //    {
        //        result = result.Or(Result<Unit>.Error(new RequiredValueMissingError("name")));
        //    }
        //    userProfile.Name = nameProperty.GetString();
        //}
        //userProfile.Age = json.GetProperty("age").GetInt32();
        //userProfile.IsSubscribed = json.GetProperty("isSubscribed").GetBoolean();
        //userProfile.Courses = json.GetProperty("courses").EnumerateArray().Select(course => course.GetString()).ToArray();

        //if (json.GetProperty("pet").ValueKind == JsonValueKind.Null)
        //{
        //    userProfile.Pet = new None<Pet>();
        //}
        //else
        //{
        //    userProfile.Pet = new Some<Pet>(new Pet
        //    {
        //        Name = json.GetProperty("pet").GetProperty("name").GetString(),
        //        Type = json.GetProperty("pet").GetProperty("type").GetString()
        //    });
        //}

        //if (json.TryGetProperty("isGay", out var isGay))
        //{
        //    userProfile.IsGay = isGay.GetBoolean();
        //}

        //if (json.TryGetProperty("favoritePet", out var favoritePet))
        //{
        //    userProfile.FavoritePet = new Pet
        //    {
        //        Name = favoritePet.GetProperty("name").GetString(),
        //        Type = favoritePet.GetProperty("type").GetString()
        //    };
        //}

        //return result.Map(() => userProfile);
    }
}
