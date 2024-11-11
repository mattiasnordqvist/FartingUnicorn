using DotNetThoughts.Results;
using System.Text.Json;
using static FartingUnicorn.MapperOptions;

namespace FartingUnicorn.Generated;

public static partial class Mappers
{
    public static Result<FartingUnicorn.Tests.MultipleFields.WithoutOptions.PATCH.BlogPost> MapToFartingUnicorn_Tests_MultipleFields_WithoutOptions_PATCH_BlogPost(JsonElement jsonElement, MapperOptions mapperOptions = null, string[] path = null)
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
            return Result<FartingUnicorn.Tests.MultipleFields.WithoutOptions.PATCH.BlogPost>.Error(new ValueHasWrongTypeError(path, "Object", jsonElement.ValueKind.ToString()));
        }
        var obj = new FartingUnicorn.Tests.MultipleFields.WithoutOptions.PATCH.BlogPost();

        List<IError> errors = new();
        var isTitlePropertyDefined = jsonElement.TryGetProperty("Title", out var jsonTitleProperty);
        if (isTitlePropertyDefined)
        {
            // type = System.String, isOption = False, isNullable = True
            if (jsonTitleProperty.ValueKind == JsonValueKind.Null)
            {
                errors.Add(new RequiredValueMissingError([.. path, "Title"]));
            }
            else if (jsonTitleProperty.ValueKind == JsonValueKind.String)
            {
                obj.Title = jsonTitleProperty.GetString();
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
        var isIsDraftPropertyDefined = jsonElement.TryGetProperty("IsDraft", out var jsonIsDraftProperty);
        if (isIsDraftPropertyDefined)
        {
            // type = System.Boolean, isOption = False, isNullable = True
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
            obj.IsDraft = null;
        }
        if(errors.Any())
        {
            return Result<FartingUnicorn.Tests.MultipleFields.WithoutOptions.PATCH.BlogPost>.Error(errors);
        }
        if(false)/*check if is option*/
        {
        }
        else
        {
            return Result<FartingUnicorn.Tests.MultipleFields.WithoutOptions.PATCH.BlogPost>.Ok(obj);
        }
        throw new NotImplementedException();
    }
}
