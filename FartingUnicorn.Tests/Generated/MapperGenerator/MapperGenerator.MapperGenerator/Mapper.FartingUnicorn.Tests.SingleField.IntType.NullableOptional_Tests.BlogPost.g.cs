using DotNetThoughts.Results;
using System.Text.Json;
using static FartingUnicorn.MapperOptions;

namespace FartingUnicorn.Generated;

public static partial class Mappers
{
    public static Result<FartingUnicorn.Tests.SingleField.IntType.NullableOptional_Tests.BlogPost> MapToFartingUnicorn_Tests_SingleField_IntType_NullableOptional_Tests_BlogPost(JsonElement jsonElement, MapperOptions mapperOptions = null, string[] path = null)
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
            return Result<FartingUnicorn.Tests.SingleField.IntType.NullableOptional_Tests.BlogPost>.Error(new ValueHasWrongTypeError(path, "Object", jsonElement.ValueKind.ToString()));
        }
        var obj = new FartingUnicorn.Tests.SingleField.IntType.NullableOptional_Tests.BlogPost();

        List<IError> errors = new();
        var isRatingPropertyDefined = jsonElement.TryGetProperty("Rating", out var jsonRatingProperty);
        if (isRatingPropertyDefined)
        {
            // type = System.Int32, isOption = True, isNullable = True
            if (jsonRatingProperty.ValueKind == JsonValueKind.Null)
            {
                obj.Rating = new None<System.Int32>();
            }
            else if (jsonRatingProperty.ValueKind == JsonValueKind.Number)
            {
                obj.Rating = new Some<int>(jsonRatingProperty.GetInt32());
            }
            else
            {
                errors.Add(new ValueHasWrongTypeError([.. path, "Rating"], "Number", jsonRatingProperty.ValueKind.ToString()));
            }
        }
        else
        {
            obj.Rating = null;
        }
        if(errors.Any())
        {
            return Result<FartingUnicorn.Tests.SingleField.IntType.NullableOptional_Tests.BlogPost>.Error(errors);
        }
        if(false)/*check if is option*/
        {
        }
        else
        {
            return Result<FartingUnicorn.Tests.SingleField.IntType.NullableOptional_Tests.BlogPost>.Ok(obj);
        }
        throw new NotImplementedException();
    }
}
