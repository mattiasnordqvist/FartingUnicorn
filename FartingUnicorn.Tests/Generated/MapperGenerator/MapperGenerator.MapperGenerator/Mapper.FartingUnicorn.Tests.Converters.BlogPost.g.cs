using DotNetThoughts.Results;
using System.Text.Json;
using static FartingUnicorn.MapperOptions;

namespace FartingUnicorn.Generated;

public static partial class Mappers
{
    public static Result<FartingUnicorn.Tests.Converters.BlogPost> MapToFartingUnicorn_Tests_Converters_BlogPost(JsonElement jsonElement, MapperOptions mapperOptions = null, string[] path = null)
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
            return Result<FartingUnicorn.Tests.Converters.BlogPost>.Error(new ValueHasWrongTypeError(path, "Object", jsonElement.ValueKind.ToString()));
        }
        var obj = new FartingUnicorn.Tests.Converters.BlogPost();

        List<IError> errors = new();
        var isIdPropertyDefined = jsonElement.TryGetProperty("Id", out var jsonIdProperty);
        if (isIdPropertyDefined)
        {
            // type = FartingUnicorn.Tests.Converters.Id, isOption = False, isNullable = False
            if (jsonIdProperty.ValueKind == JsonValueKind.Null)
            {
                errors.Add(new RequiredValueMissingError([.. path, "Id"]));
            }
            else if (mapperOptions.TryGetConverter(typeof(FartingUnicorn.Tests.Converters.Id), out IConverter customConverter))
            {
                if (jsonIdProperty.ValueKind != customConverter.ExpectedJsonValueKind)
                {
                    errors.Add(new ValueHasWrongTypeError([.. path, "Id"], customConverter.ExpectedJsonValueKind.ToString(), jsonIdProperty.ValueKind.ToString()));
                }
                else
                {
                    var result = customConverter.Convert(typeof(FartingUnicorn.Tests.Converters.Id), jsonIdProperty, mapperOptions, [.. path, "Id"]);
                    if (result.Success)
                    {
                        obj.Id = result.Map(x => (FartingUnicorn.Tests.Converters.Id)x).Value;
                    }
                    else
                    {
                        errors.AddRange(result.Errors.Select(x => new MappingError([.. path, "Id"], x.Message)).ToArray());
                    }
                }
            }
        }
        else
        {
            errors.Add(new RequiredPropertyMissingError([.. path, "Id"]));
        }
        var isTitlePropertyDefined = jsonElement.TryGetProperty("Title", out var jsonTitleProperty);
        if (isTitlePropertyDefined)
        {
            // type = System.String, isOption = False, isNullable = False
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
            errors.Add(new RequiredPropertyMissingError([.. path, "Title"]));
        }
        if(errors.Any())
        {
            return Result<FartingUnicorn.Tests.Converters.BlogPost>.Error(errors);
        }
        if(false)/*check if is option*/
        {
        }
        else
        {
            return Result<FartingUnicorn.Tests.Converters.BlogPost>.Ok(obj);
        }
        throw new NotImplementedException();
    }
}
