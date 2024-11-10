using DotNetThoughts.Results;
using System.Text.Json;
using static FartingUnicorn.MapperOptions;

namespace FartingUnicorn.Generated;

public static partial class Mappers
{
    public static Result<FartingUnicorn.Tests.SingleField.BoolType.NonNullableNonOptional_Tests.BlogPost> MapToFartingUnicorn_Tests_SingleField_BoolType_NonNullableNonOptional_Tests_BlogPost(JsonElement jsonElement, MapperOptions mapperOptions = null, string[] path = null)
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
            return Result<FartingUnicorn.Tests.SingleField.BoolType.NonNullableNonOptional_Tests.BlogPost>.Error(new ValueHasWrongTypeError(path, "Object", jsonElement.ValueKind.ToString()));
        }
        var obj = new FartingUnicorn.Tests.SingleField.BoolType.NonNullableNonOptional_Tests.BlogPost();

        List<IError> errors = new();
        var isIsDraftPropertyDefined = jsonElement.TryGetProperty("IsDraft", out var jsonIsDraftProperty);
        if (isIsDraftPropertyDefined)
        {
            // type = System.Boolean, isOption = False, isNullable = False
            if (jsonIsDraftProperty.ValueKind == JsonValueKind.Null)
            {
                errors.Add(new RequiredValueMissingError([.. path, "IsDraft"]));
            }
            else if (jsonIsDraftProperty.ValueKind == JsonValueKind.True || jsonIsDraftProperty.ValueKind == JsonValueKind.False)
            {
                obj.IsDraft = jsonIsDraftProperty.GetBoolean();
            }
            else
            {
                errors.Add(new ValueHasWrongTypeError([.. path, "IsDraft"], "Boolean", jsonIsDraftProperty.ValueKind.ToString()));
            }
        }
        else
        {
            errors.Add(new RequiredPropertyMissingError([.. path, "IsDraft"]));
        }
        if(errors.Any())
        {
            return Result<FartingUnicorn.Tests.SingleField.BoolType.NonNullableNonOptional_Tests.BlogPost>.Error(errors);
        }
        if(false)/*check if is option*/
        {
        }
        else
        {
            return Result<FartingUnicorn.Tests.SingleField.BoolType.NonNullableNonOptional_Tests.BlogPost>.Ok(obj);
        }
        throw new NotImplementedException();
    }
}
