﻿using DotNetThoughts.Results;
using System.Text.Json;
using static FartingUnicorn.MapperOptions;

namespace FartingUnicorn.Generated;

public static partial class Mappers
{
    public static Result<FartingUnicorn.Benchmarks.UserProfile> MapToFartingUnicorn_Benchmarks_UserProfile(JsonElement jsonElement, MapperOptions mapperOptions = null, string[] path = null)
    {
        if (mapperOptions is null)
        {
            mapperOptions = new MapperOptions();
        }
        if (path is null)
        {
            path = ["$"];
        }
        if (jsonElement.ValueKind != JsonValueKind.Object)
        {
            return Result<FartingUnicorn.Benchmarks.UserProfile>.Error(new ValueHasWrongTypeError(path, "Object", jsonElement.ValueKind.ToString()));
        }
        var obj = new FartingUnicorn.Benchmarks.UserProfile();

        List<IError> errors = new();
        var isNamePropertyDefined = jsonElement.TryGetProperty("Name", out var jsonNameProperty);
        if (isNamePropertyDefined)
        {
            // type = System.String, isOption = False, isNullable = False
            if (jsonNameProperty.ValueKind == JsonValueKind.Null)
            {
                errors.Add(new RequiredValueMissingError([.. path, "Name"]));
            }
            else if (jsonNameProperty.ValueKind == JsonValueKind.String)
            {
                obj.Name = jsonNameProperty.GetString();
            }
            else
            {
                errors.Add(new ValueHasWrongTypeError([.. path, "Name"], "String", jsonNameProperty.ValueKind.ToString()));
            }
        }
        else
        {
            errors.Add(new RequiredPropertyMissingError([.. path, "Name"]));
        }
        if(errors.Any())
        {
            return Result<FartingUnicorn.Benchmarks.UserProfile>.Error(errors);
        }
        if(false)/*check if is option*/
        {
        }
        else
        {
            return Result<FartingUnicorn.Benchmarks.UserProfile>.Ok(obj);
        }
        throw new NotImplementedException();
    }
}
