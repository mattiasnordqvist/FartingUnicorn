using DotNetThoughts.Results;
using System.Text.Json;
using static FartingUnicorn.MapperOptions;

namespace FartingUnicorn.Generated;

public static partial class Mappers
{
    public static Result<FartingUnicorn.Tests.Objects.Nullable.BlogPost> MapToFartingUnicorn_Tests_Objects_Nullable_BlogPost(JsonElement jsonElement, MapperOptions mapperOptions = null, string[] path = null)
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
            return Result<FartingUnicorn.Tests.Objects.Nullable.BlogPost>.Error(new ValueHasWrongTypeError(path, "Object", jsonElement.ValueKind.ToString()));
        }
        var obj = new FartingUnicorn.Tests.Objects.Nullable.BlogPost();

        List<IError> errors = new();
        var isAuthorPropertyDefined = jsonElement.TryGetProperty("Author", out var jsonAuthorProperty);
        if (isAuthorPropertyDefined)
        {
            // type = FartingUnicorn.Tests.Objects.Nullable.Author, isOption = False, isNullable = True
            if (jsonAuthorProperty.ValueKind == JsonValueKind.Null)
            {
                errors.Add(new RequiredValueMissingError([.. path, "Author"]));
            }
            else if (mapperOptions.TryGetConverter(typeof(FartingUnicorn.Tests.Objects.Nullable.Author), out IConverter customConverter))
            {
                if (jsonAuthorProperty.ValueKind != customConverter.ExpectedJsonValueKind)
                {
                    errors.Add(new ValueHasWrongTypeError([.. path, "Author"], customConverter.ExpectedJsonValueKind.ToString(), jsonAuthorProperty.ValueKind.ToString()));
                }
                else
                {
                    var result = customConverter.Convert(typeof(FartingUnicorn.Tests.Objects.Nullable.Author), jsonAuthorProperty, mapperOptions, [.. path, "Author"]);
                    if (result.Success)
                    {
                        obj.Author = result.Map(x => (FartingUnicorn.Tests.Objects.Nullable.Author)x).Value;
                    }
                    else
                    {
                        errors.AddRange(result.Errors.Select(x => new MappingError([.. path, "Author"], x.Message)).ToArray());
                    }
                }
            }
        }
        else
        {
            obj.Author = null;
        }
        if(errors.Any())
        {
            return Result<FartingUnicorn.Tests.Objects.Nullable.BlogPost>.Error(errors);
        }
        if(false)/*check if is option*/
        {
        }
        else
        {
            return Result<FartingUnicorn.Tests.Objects.Nullable.BlogPost>.Ok(obj);
        }
        throw new NotImplementedException();
    }
}
