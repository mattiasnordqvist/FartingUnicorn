using DotNetThoughts.Results;
using System.Text.Json;

namespace FartingUnicorn.Generated;

public static partial class Mappers
{
    public static Result<FartingUnicorn.Tests.SingleField.IntType.NonNullableOptional_Tests.BlogPost> MapToFartingUnicorn_Tests_SingleField_IntType_NonNullableOptional_Tests_BlogPost(JsonElement jsonElement, string[] path = null)
    {
        if(path is null)
        {
            path = ["$"];
        }
        /*object*/
        {
            if (jsonElement.ValueKind != JsonValueKind.Object)
            {
                return Result<FartingUnicorn.Tests.SingleField.IntType.NonNullableOptional_Tests.BlogPost>.Error(new ValueHasWrongTypeError(path, "Object", jsonElement.ValueKind.ToString()));
            }
        }
        var obj = new FartingUnicorn.Tests.SingleField.IntType.NonNullableOptional_Tests.BlogPost();

        List<IError> errors = new();
        var isRatingPropertyDefined = jsonElement.TryGetProperty("Rating", out var jsonRatingProperty);
        if (isRatingPropertyDefined)
        {
            // type = Int32, isOption = True, isNullable = False
            if (jsonRatingProperty.ValueKind == JsonValueKind.Null)
            {
                obj.Rating = new None<Int32>();
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
            errors.Add(new RequiredPropertyMissingError([.. path, "Rating"]));
        }
        if(errors.Any())
        {
            return Result<FartingUnicorn.Tests.SingleField.IntType.NonNullableOptional_Tests.BlogPost>.Error(errors);
        }
        if(false)/*check if is option*/
        {
        }
        else
        {
            return Result<FartingUnicorn.Tests.SingleField.IntType.NonNullableOptional_Tests.BlogPost>.Ok(obj);
        }
        throw new NotImplementedException();
    }
}
