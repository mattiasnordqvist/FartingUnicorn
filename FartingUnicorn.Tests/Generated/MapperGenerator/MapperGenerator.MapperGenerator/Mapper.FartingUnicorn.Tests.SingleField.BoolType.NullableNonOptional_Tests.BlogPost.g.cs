﻿using DotNetThoughts.Results;
using System.Text.Json;

namespace FartingUnicorn.Generated;

public static partial class Mappers
{
    public static Result<FartingUnicorn.Tests.SingleField.BoolType.NullableNonOptional_Tests.BlogPost> MapToFartingUnicorn_Tests_SingleField_BoolType_NullableNonOptional_Tests_BlogPost(JsonElement jsonElement, string[] path = null)
    {
        if(path is null)
        {
            path = ["$"];
        }
        /*object*/
        {
            if (jsonElement.ValueKind != JsonValueKind.Object)
            {
                return Result<FartingUnicorn.Tests.SingleField.BoolType.NullableNonOptional_Tests.BlogPost>.Error(new ValueHasWrongTypeError(path, "Object", jsonElement.ValueKind.ToString()));
            }
        }
        var obj = new FartingUnicorn.Tests.SingleField.BoolType.NullableNonOptional_Tests.BlogPost();

        List<IError> errors = new();
        var isIsDraftPropertyDefined = jsonElement.TryGetProperty("IsDraft", out var jsonIsDraftProperty);
        if (isIsDraftPropertyDefined)
        {
            // type = Nullable, isOption = False, isNullable = True
            if (jsonIsDraftProperty.ValueKind == JsonValueKind.Null)
            {
                errors.Add(new RequiredValueMissingError([.. path, "IsDraft"]));
            }
        }
        else
        {
            obj.IsDraft = null;
        }
        if(errors.Any())
        {
            return Result<FartingUnicorn.Tests.SingleField.BoolType.NullableNonOptional_Tests.BlogPost>.Error(errors);
        }
        if(false)/*check if is option*/
        {
        }
        else
        {
            return Result<FartingUnicorn.Tests.SingleField.BoolType.NullableNonOptional_Tests.BlogPost>.Ok(obj);
        }
        throw new NotImplementedException();
    }
}
