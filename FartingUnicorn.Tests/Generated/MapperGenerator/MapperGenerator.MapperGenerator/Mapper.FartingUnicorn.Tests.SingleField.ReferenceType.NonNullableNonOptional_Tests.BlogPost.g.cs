using DotNetThoughts.Results;
using System.Text.Json;

namespace FartingUnicorn.Generated;

public static partial class Mappers
{
    public static Result<FartingUnicorn.Tests.SingleField.ReferenceType.NonNullableNonOptional_Tests.BlogPost> MapToFartingUnicorn_Tests_SingleField_ReferenceType_NonNullableNonOptional_Tests_BlogPost(JsonElement jsonElement, string[] path = null)
    {
        /*object*/
        {
            if (jsonElement.ValueKind != JsonValueKind.Object)
            {
                return Result<FartingUnicorn.Tests.SingleField.ReferenceType.NonNullableNonOptional_Tests.BlogPost>.Error(new ValueHasWrongTypeError(path, "Object", jsonElement.ValueKind.ToString()));
            }
        }
        var obj = new FartingUnicorn.Tests.SingleField.ReferenceType.NonNullableNonOptional_Tests.BlogPost();

        Result<Unit> compositeResult = UnitResult.Ok;
        var isTitlePropertyDefined = jsonElement.TryGetProperty("Title", out var jsonTitleProperty);
        if (isTitlePropertyDefined)
        {
            // String
            var mapResult = MapString(jsonTitleProperty, /*mapperOptions,*/ [.. path, Title]);
            if (mapResult.Success)
            {
                obj.Title = mapResult.Value;
            }
            else
            {
                compositeResult = compositeResult.Or(mapResult);
            }
        }
        else
        {
        }
        if(!compositeResult.Success)
        {
            return Result<FartingUnicorn.Tests.SingleField.ReferenceType.NonNullableNonOptional_Tests.BlogPost>.Error(compositeResult.Errors);
        }
        if(false)/*check if is option*/
        {
        }
        else
        {
            return Result<FartingUnicorn.Tests.SingleField.ReferenceType.NonNullableNonOptional_Tests.BlogPost>.Ok(obj);
        }
        throw new NotImplementedException();
    }
}
