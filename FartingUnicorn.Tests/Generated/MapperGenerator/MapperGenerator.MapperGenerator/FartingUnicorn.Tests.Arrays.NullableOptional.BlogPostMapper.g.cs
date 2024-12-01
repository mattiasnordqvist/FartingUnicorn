﻿// <auto-generated/>
using DotNetThoughts.Results;
using System.Text.Json;
using static FartingUnicorn.MapperOptions;

namespace FartingUnicorn.Tests;

// ClassName: BlogPost
// FullName: FartingUnicorn.Tests.Arrays.NullableOptional.BlogPost
// Namespace: FartingUnicorn.Tests
// Properties: 1
// HasCreateMapperAttribute: False
// ClassPath: Arrays, NullableOptional

// Property 0
// Name: Comments
// CompleteType: FartingUnicorn.Option<FartingUnicorn.Tests.Arrays.NullableOptional.Comment[]>?
// IsArray: True
// IsObject: False
// IsNullable: True
// IsOption: True
// RawType: FartingUnicorn.Tests.Arrays.NullableOptional.Comment[]
// ArrayElemCompleteType: FartingUnicorn.Tests.Arrays.NullableOptional.Comment
// IsArrayElemArray: False
// IsArrayElemObject: True
// IsArrayElemOption: False
// ArrayElemRawType: FartingUnicorn.Tests.Arrays.NullableOptional.Comment


public partial class Arrays
{
    public partial class NullableOptional
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
                var isCommentsPropertyDefined = jsonElement.TryGetProperty("Comments", out var jsonCommentsProperty);
                if (isCommentsPropertyDefined)
                {
                    if (jsonCommentsProperty.ValueKind == JsonValueKind.Null)
                    {
                        obj.Comments = new None<FartingUnicorn.Tests.Arrays.NullableOptional.Comment[]>();
                    }
                    else if (jsonCommentsProperty.ValueKind == JsonValueKind.Array)
                    {
                        var array = new FartingUnicorn.Tests.Arrays.NullableOptional.Comment[jsonCommentsProperty.GetArrayLength()];
                        for(int i = 0; i < jsonCommentsProperty.GetArrayLength(); i++)
                        {
                            var result = FartingUnicorn.Tests.Arrays.NullableOptional.Comment.MapFromJson(jsonCommentsProperty[i], mapperOptions, [.. path, "Comments", i.ToString()]);
                            if (result.Success)
                            {
                                array.SetValue(result.Value, i);
                            }
                            else
                            {
                                errors.AddRange(result.Errors.ToArray());
                            }
                        }
                        obj.Comments = new Some<FartingUnicorn.Tests.Arrays.NullableOptional.Comment[]>(array);
                    }
                    else
                    {
                        errors.Add(new ValueHasWrongTypeError([.. path, "Comments"], "Array", jsonCommentsProperty.ValueKind.ToString()));
                    }
                }
                else
                {
                    obj.Comments = null;
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