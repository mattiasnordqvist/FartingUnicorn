﻿// <auto-generated/>
using DotNetThoughts.Results;
using System.Text.Json;
using FartingUnicorn;

namespace FartingUnicorn.Tests;

// ClassName: BlogPost
// FullName: FartingUnicorn.Tests.MultipleFields.WithoutOptions.PATCH.BlogPost
// Namespace: FartingUnicorn.Tests
// Properties: 2
// HasCreateMapperAttribute: False
// ClassPath: MultipleFields, WithoutOptions, PATCH

// Property 0
// Name: Title
// CompleteType: string?
// IsArray: False
// IsObject: False
// IsNullable: True
// IsOption: False
// RawType: System.String

// Property 1
// Name: IsDraft
// CompleteType: bool?
// IsArray: False
// IsObject: False
// IsNullable: True
// IsOption: False
// RawType: System.Boolean


public partial class MultipleFields
{
    public partial class WithoutOptions
    {
        public partial class PATCH
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
                    var obj = new BlogPost();

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
    }
}
