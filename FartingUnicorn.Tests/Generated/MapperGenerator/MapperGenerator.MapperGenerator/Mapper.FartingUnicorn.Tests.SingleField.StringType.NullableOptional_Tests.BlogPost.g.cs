﻿using DotNetThoughts.Results;
using System.Text.Json;
using static FartingUnicorn.MapperOptions;

namespace FartingUnicorn.Generated;

public static partial class Mappers
{
    public static Result<FartingUnicorn.Tests.SingleField.StringType.NullableOptional_Tests.BlogPost> MapToFartingUnicorn_Tests_SingleField_StringType_NullableOptional_Tests_BlogPost(JsonElement jsonElement, MapperOptions mapperOptions = null, string[] path = null)
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
            return Result<FartingUnicorn.Tests.SingleField.StringType.NullableOptional_Tests.BlogPost>.Error(new ValueHasWrongTypeError(path, "Object", jsonElement.ValueKind.ToString()));
        }
        var obj = new FartingUnicorn.Tests.SingleField.StringType.NullableOptional_Tests.BlogPost();

        List<IError> errors = new();
        var isTitlePropertyDefined = jsonElement.TryGetProperty("Title", out var jsonTitleProperty);
        if (isTitlePropertyDefined)
        {
            // type = System.String, isOption = True, isNullable = True
            if (jsonTitleProperty.ValueKind == JsonValueKind.Null)
            {
                obj.Title = new None<System.String>();
            }
            else if (jsonTitleProperty.ValueKind == JsonValueKind.String)
            {
                obj.Title = new Some<string>(jsonTitleProperty.GetString());
            }
            else
            {
                errors.Add(new ValueHasWrongTypeError([.. path, "Title"], "String", jsonTitleProperty.ValueKind.ToString()));
            }
        }
        else
        {
            obj.Title = null;
        }
        if(errors.Any())
        {
            return Result<FartingUnicorn.Tests.SingleField.StringType.NullableOptional_Tests.BlogPost>.Error(errors);
        }
        if(false)/*check if is option*/
        {
        }
        else
        {
            return Result<FartingUnicorn.Tests.SingleField.StringType.NullableOptional_Tests.BlogPost>.Ok(obj);
        }
        throw new NotImplementedException();
    }
}