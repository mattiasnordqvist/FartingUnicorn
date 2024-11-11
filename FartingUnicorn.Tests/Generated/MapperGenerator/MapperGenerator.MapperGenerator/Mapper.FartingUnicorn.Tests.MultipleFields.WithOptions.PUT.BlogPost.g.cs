using DotNetThoughts.Results;
using System.Text.Json;
using static FartingUnicorn.MapperOptions;

namespace FartingUnicorn.Generated;

public static partial class Mappers
{
    public static Result<FartingUnicorn.Tests.MultipleFields.WithOptions.PUT.BlogPost> MapToFartingUnicorn_Tests_MultipleFields_WithOptions_PUT_BlogPost(JsonElement jsonElement, MapperOptions mapperOptions = null, string[] path = null)
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
            return Result<FartingUnicorn.Tests.MultipleFields.WithOptions.PUT.BlogPost>.Error(new ValueHasWrongTypeError(path, "Object", jsonElement.ValueKind.ToString()));
        }
        var obj = new FartingUnicorn.Tests.MultipleFields.WithOptions.PUT.BlogPost();

        List<IError> errors = new();
        var isCategoryPropertyDefined = jsonElement.TryGetProperty("Category", out var jsonCategoryProperty);
        if (isCategoryPropertyDefined)
        {
            // type = System.String, isOption = True, isNullable = False
            if (jsonCategoryProperty.ValueKind == JsonValueKind.Null)
            {
                obj.Category = new None<System.String>();
            }
            else if (jsonCategoryProperty.ValueKind == JsonValueKind.String)
            {
                obj.Category = new Some<string>(jsonCategoryProperty.GetString());
            }
            else
            {
                errors.Add(new ValueHasWrongTypeError([.. path, "Category"], "String", jsonCategoryProperty.ValueKind.ToString()));
            }
        }
        else
        {
            errors.Add(new RequiredPropertyMissingError([.. path, "Category"]));
        }
        var isRatingPropertyDefined = jsonElement.TryGetProperty("Rating", out var jsonRatingProperty);
        if (isRatingPropertyDefined)
        {
            // type = System.Int32, isOption = True, isNullable = False
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
            errors.Add(new RequiredPropertyMissingError([.. path, "Rating"]));
        }
        if(errors.Any())
        {
            return Result<FartingUnicorn.Tests.MultipleFields.WithOptions.PUT.BlogPost>.Error(errors);
        }
        if(false)/*check if is option*/
        {
        }
        else
        {
            return Result<FartingUnicorn.Tests.MultipleFields.WithOptions.PUT.BlogPost>.Ok(obj);
        }
        throw new NotImplementedException();
    }
}
