﻿// <auto-generated/>
using DotNetThoughts.Results;
using System.Text.Json;
using FartingUnicorn;

namespace FartingUnicorn.Tests;

// ClassName: BlogPost
// FullName: FartingUnicorn.Tests.SingleField.BoolType.NullableOptional_Tests.BlogPost
// Namespace: FartingUnicorn.Tests
// Properties: 1
// HasCreateMapperAttribute: False
// ClassPath: SingleField, BoolType, NullableOptional_Tests

// Property 0
// Name: IsDraft
// CompleteType: FartingUnicorn.Option<bool>?
// IsArray: False
// IsObject: False
// IsNullable: True
// IsOption: True
// RawType: System.Boolean


public partial class SingleField
{
    public partial class BoolType
    {
        public partial class NullableOptional_Tests
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
                    var p_IsDraft = default(FartingUnicorn.Option<bool>?);

                    List<IError> errors = new();
                    var isIsDraftPropertyDefined = jsonElement.TryGetProperty("IsDraft", out var jsonIsDraftProperty);
                    if (isIsDraftPropertyDefined)
                    {
                        if (jsonIsDraftProperty.ValueKind == JsonValueKind.Null)
                        {
                            p_IsDraft = new None<System.Boolean>();
                        }
                        else if (jsonIsDraftProperty.ValueKind == JsonValueKind.True || jsonIsDraftProperty.ValueKind == JsonValueKind.False)
                        {
                            p_IsDraft = new Some<bool>(jsonIsDraftProperty.GetBoolean());
                        }
                        else
                        {
                            errors.Add(new ValueHasWrongTypeError([.. path, "IsDraft"], "Boolean", jsonIsDraftProperty.ValueKind.ToString()));
                        }
                    }
                    else
                    {
                        p_IsDraft = null;
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
                        obj.IsDraft = p_IsDraft;
                        return Result<BlogPost>.Ok(obj);
                    }
                    throw new NotImplementedException();
                }
            }
        }
    }
}
