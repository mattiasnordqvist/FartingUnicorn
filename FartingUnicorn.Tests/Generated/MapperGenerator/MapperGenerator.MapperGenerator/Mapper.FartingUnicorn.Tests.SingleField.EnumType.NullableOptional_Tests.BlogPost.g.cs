﻿using DotNetThoughts.Results;
using System.Text.Json;
using static FartingUnicorn.MapperOptions;

namespace FartingUnicorn.Generated;

public static partial class Mappers
{
    public static Result<FartingUnicorn.Tests.SingleField.EnumType.NullableOptional_Tests.BlogPost> MapToFartingUnicorn_Tests_SingleField_EnumType_NullableOptional_Tests_BlogPost(JsonElement jsonElement, MapperOptions mapperOptions = null, string[] path = null)
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
            return Result<FartingUnicorn.Tests.SingleField.EnumType.NullableOptional_Tests.BlogPost>.Error(new ValueHasWrongTypeError(path, "Object", jsonElement.ValueKind.ToString()));
        }
        var obj = new FartingUnicorn.Tests.SingleField.EnumType.NullableOptional_Tests.BlogPost();

        List<IError> errors = new();
        var isStatusPropertyDefined = jsonElement.TryGetProperty("Status", out var jsonStatusProperty);
        if (isStatusPropertyDefined)
        {
            // type = FartingUnicorn.Tests.SingleField.EnumType.NullableOptional_Tests.BlogPost.BlogPostStatus, isOption = True, isNullable = True
            if (jsonStatusProperty.ValueKind == JsonValueKind.Null)
            {
                obj.Status = new None<FartingUnicorn.Tests.SingleField.EnumType.NullableOptional_Tests.BlogPost.BlogPostStatus>();
            }
            else if (mapperOptions.TryGetConverter(typeof(FartingUnicorn.Tests.SingleField.EnumType.NullableOptional_Tests.BlogPost.BlogPostStatus), out IConverter customConverter))
            {
                if (jsonStatusProperty.ValueKind != customConverter.ExpectedJsonValueKind)
                {
                    errors.Add(new ValueHasWrongTypeError([.. path, "Status"], customConverter.ExpectedJsonValueKind.ToString(), jsonStatusProperty.ValueKind.ToString()));
                }
                else
                {
                    var result = customConverter.Convert(typeof(FartingUnicorn.Tests.SingleField.EnumType.NullableOptional_Tests.BlogPost.BlogPostStatus), jsonStatusProperty, mapperOptions, [.. path, "Status"]);
                    if (result.Success)
                    {
                        obj.Status = new Some<FartingUnicorn.Tests.SingleField.EnumType.NullableOptional_Tests.BlogPost.BlogPostStatus>(result.Map(x => (FartingUnicorn.Tests.SingleField.EnumType.NullableOptional_Tests.BlogPost.BlogPostStatus)x).Value);
                    }
                    else
                    {
                        errors.AddRange(result.Errors.Select(x => new MappingError([.. path, "Status"], x.Message)).ToArray());
                    }
                }
            }
        }
        else
        {
            obj.Status = null;
        }
        if(errors.Any())
        {
            return Result<FartingUnicorn.Tests.SingleField.EnumType.NullableOptional_Tests.BlogPost>.Error(errors);
        }
        if(false)/*check if is option*/
        {
        }
        else
        {
            return Result<FartingUnicorn.Tests.SingleField.EnumType.NullableOptional_Tests.BlogPost>.Ok(obj);
        }
        throw new NotImplementedException();
    }
}
