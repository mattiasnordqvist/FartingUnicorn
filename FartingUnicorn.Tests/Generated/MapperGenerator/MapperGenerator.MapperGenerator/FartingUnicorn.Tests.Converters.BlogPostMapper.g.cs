﻿// <auto-generated/>
using DotNetThoughts.Results;
using System.Text.Json;
using static FartingUnicorn.MapperOptions;

namespace FartingUnicorn.Tests;

// ClassName: BlogPost
// FullName: FartingUnicorn.Tests.Converters.BlogPost
// Namespace: FartingUnicorn.Tests
// Properties: 2
// HasCreateMapperAttribute: False
// ClassPath: Converters

// Property 0
// Name: Id
// TypeName: FartingUnicorn.Tests.Converters.Id
// IsArray: False
// IsReferenceType: True
// IsNullable: False
// IsOption: False
// EffectiveType: FartingUnicorn.Tests.Converters.Id

// Property 1
// Name: Title
// TypeName: string
// IsArray: False
// IsReferenceType: True
// IsNullable: False
// IsOption: False
// EffectiveType: System.String


public partial class Converters
{
    public partial class BlogPost
    {
        // hello
        public static Result<BlogPost> MapFromJson(JsonElement jsonElement, MapperOptions mapperOptions = null, string[] path = null)
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
                return Result<BlogPost>.Error(new ValueHasWrongTypeError(path, "Object", jsonElement.ValueKind.ToString()));
            }
            var obj = new BlogPost();

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
                else
                {
                    if (jsonIdProperty.ValueKind == JsonValueKind.Object)
                    {
                        var result = FartingUnicorn.Tests.Converters.Id.MapFromJson(jsonIdProperty, mapperOptions, [.. path, "Id"]);
                        if (result.Success)
                        {
                            obj.Id = result.Value;
                        }
                        else
                        {
                            errors.AddRange(result.Errors.ToArray());
                        }
                    }
                    else
                    {
                        errors.Add(new ValueHasWrongTypeError([.. path, "Id"], "Object", jsonIdProperty.ValueKind.ToString()));
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
                // type = string, isOption = False, isNullable = False
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
                return Result<BlogPost>.Error(errors);
            }
            if(false)/*check if is option*/
            {
            }
            else
            {
                return Result<BlogPost>.Ok(obj);
            }
            throw new NotImplementedException();
        }
    }
}
