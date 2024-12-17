﻿// <auto-generated/>
using DotNetThoughts.Results;
using System.Text.Json;
using FartingUnicorn;

namespace FartingUnicorn.Tests;

// ClassName: BlogPost
// FullName: FartingUnicorn.Tests.Enums.NotNullableNotOptionable.BlogPost
// Namespace: FartingUnicorn.Tests
// Properties: 2
// HasCreateMapperAttribute: False
// ClassPath: Enums, NotNullableNotOptionable

// Property 0
// Name: Title
// CompleteType: string
// IsArray: False
// IsObject: False
// IsNullable: False
// IsOption: False
// RawType: System.String

// Property 1
// Name: Status
// CompleteType: FartingUnicorn.Tests.Enums.NotNullableNotOptionable.BlogPostStatus
// IsArray: False
// IsObject: False
// IsNullable: False
// IsOption: False
// RawType: FartingUnicorn.Tests.Enums.NotNullableNotOptionable.BlogPostStatus


public partial class Enums
{
    public partial class NotNullableNotOptionable
    {
        public partial class BlogPost
        {
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
                var p_Title = default(string);
                var p_Status = default(FartingUnicorn.Tests.Enums.NotNullableNotOptionable.BlogPostStatus);

                List<IError> errors = new();
                var isTitlePropertyDefined = jsonElement.TryGetProperty("Title", out var jsonTitleProperty);
                if (isTitlePropertyDefined)
                {
                    if (jsonTitleProperty.ValueKind == JsonValueKind.Null)
                    {
                        errors.Add(new RequiredValueMissingError([.. path, "Title"]));
                    }
                    else if (jsonTitleProperty.ValueKind == JsonValueKind.String)
                    {
                        p_Title = jsonTitleProperty.GetString();
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
                var isStatusPropertyDefined = jsonElement.TryGetProperty("Status", out var jsonStatusProperty);
                if (isStatusPropertyDefined)
                {
                    if (jsonStatusProperty.ValueKind == JsonValueKind.Null)
                    {
                        errors.Add(new RequiredValueMissingError([.. path, "Status"]));
                    }
                    else if (mapperOptions.TryGetConverter(typeof(FartingUnicorn.Tests.Enums.NotNullableNotOptionable.BlogPostStatus), out IConverter customConverter))
                    {
                        if (jsonStatusProperty.ValueKind != customConverter.ExpectedJsonValueKind)
                        {
                            errors.Add(new ValueHasWrongTypeError([.. path, "Status"], customConverter.ExpectedJsonValueKind.ToString(), jsonStatusProperty.ValueKind.ToString()));
                        }
                        else
                        {
                            var result = customConverter.Convert(typeof(FartingUnicorn.Tests.Enums.NotNullableNotOptionable.BlogPostStatus), jsonStatusProperty, mapperOptions, [.. path, "Status"]);
                            if (result.Success)
                            {
                                p_Status = result.Map(x => (FartingUnicorn.Tests.Enums.NotNullableNotOptionable.BlogPostStatus)x).Value;
                            }
                            else
                            {
                                errors.AddRange(result.Errors.Select(x => new MappingError([.. path, "Status"], x.Message)).ToArray());
                            }
                        }
                    }
                }
                else
                {
                    errors.Add(new RequiredPropertyMissingError([.. path, "Status"]));
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
                    var obj = new BlogPost();
                    obj.Title = p_Title;
                    obj.Status = p_Status;
                    return Result<BlogPost>.Ok(obj);
                }
                throw new NotImplementedException();
            }
        }
    }
}
