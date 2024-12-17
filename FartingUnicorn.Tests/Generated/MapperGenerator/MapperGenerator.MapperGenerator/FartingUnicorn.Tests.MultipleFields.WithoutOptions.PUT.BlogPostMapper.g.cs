﻿// <auto-generated/>
using DotNetThoughts.Results;
using System.Text.Json;
using FartingUnicorn;

namespace FartingUnicorn.Tests;

// ClassName: BlogPost
// FullName: FartingUnicorn.Tests.MultipleFields.WithoutOptions.PUT.BlogPost
// Namespace: FartingUnicorn.Tests
// Properties: 2
// HasCreateMapperAttribute: False
// ClassPath: MultipleFields, WithoutOptions, PUT

// Property 0
// Name: Title
// CompleteType: string
// IsArray: False
// IsObject: False
// IsNullable: False
// IsOption: False
// RawType: System.String

// Property 1
// Name: IsDraft
// CompleteType: bool
// IsArray: False
// IsObject: False
// IsNullable: False
// IsOption: False
// RawType: System.Boolean


public partial class MultipleFields
{
    public partial class WithoutOptions
    {
        public partial class PUT
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
                    var p_IsDraft = default(bool);

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
                    var isIsDraftPropertyDefined = jsonElement.TryGetProperty("IsDraft", out var jsonIsDraftProperty);
                    if (isIsDraftPropertyDefined)
                    {
                        if (jsonIsDraftProperty.ValueKind == JsonValueKind.Null)
                        {
                            errors.Add(new RequiredValueMissingError([.. path, "IsDraft"]));
                        }
                        else if (jsonIsDraftProperty.ValueKind == JsonValueKind.True || jsonIsDraftProperty.ValueKind == JsonValueKind.False)
                        {
                            p_IsDraft = jsonIsDraftProperty.GetBoolean();
                        }
                        else
                        {
                            errors.Add(new ValueHasWrongTypeError([.. path, "IsDraft"], "Boolean", jsonIsDraftProperty.ValueKind.ToString()));
                        }
                    }
                    else
                    {
                        errors.Add(new RequiredPropertyMissingError([.. path, "IsDraft"]));
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
                        obj.IsDraft = p_IsDraft;
                        return Result<BlogPost>.Ok(obj);
                    }
                    throw new NotImplementedException();
                }
            }
        }
    }
}
