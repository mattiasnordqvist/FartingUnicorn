﻿// <auto-generated/>
using DotNetThoughts.Results;
using System.Text.Json;
using FartingUnicorn;

namespace FartingUnicorn.Tests;

// ClassName: BlogPost
// FullName: FartingUnicorn.Tests.Arrays.Nullable.BlogPost
// Namespace: FartingUnicorn.Tests
// Properties: 1
// HasCreateMapperAttribute: False
// ClassPath: Arrays, Nullable

// Property 0
// Name: Comments
// CompleteType: FartingUnicorn.Tests.Arrays.Nullable.Comment[]?
// IsArray: True
// IsObject: False
// IsNullable: True
// IsOption: False
// RawType: FartingUnicorn.Tests.Arrays.Nullable.Comment[]
// ArrayElemCompleteType: FartingUnicorn.Tests.Arrays.Nullable.Comment
// IsArrayElemArray: False
// IsArrayElemObject: True
// IsArrayElemOption: False
// ArrayElemRawType: FartingUnicorn.Tests.Arrays.Nullable.Comment


public partial class Arrays
{
    public partial class Nullable
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
                var p_Comments = default(FartingUnicorn.Tests.Arrays.Nullable.Comment[]?);

                List<IError> errors = new();
                var isCommentsPropertyDefined = jsonElement.TryGetProperty("Comments", out var jsonCommentsProperty);
                if (isCommentsPropertyDefined)
                {
                    if (jsonCommentsProperty.ValueKind == JsonValueKind.Null)
                    {
                        errors.Add(new RequiredValueMissingError([.. path, "Comments"]));
                    }
                    else if (jsonCommentsProperty.ValueKind == JsonValueKind.Array)
                    {
                        var array = new FartingUnicorn.Tests.Arrays.Nullable.Comment[jsonCommentsProperty.GetArrayLength()];
                        for(int i = 0; i < jsonCommentsProperty.GetArrayLength(); i++)
                        {
                            var result = FartingUnicorn.Tests.Arrays.Nullable.Comment.MapFromJson(jsonCommentsProperty[i], mapperOptions, [.. path, "Comments", i.ToString()]);
                            if (result.Success)
                            {
                                array.SetValue(result.Value, i);
                            }
                            else
                            {
                                errors.AddRange(result.Errors.ToArray());
                            }
                        }
                        p_Comments = array;
                    }
                    else
                    {
                        errors.Add(new ValueHasWrongTypeError([.. path, "Comments"], "Array", jsonCommentsProperty.ValueKind.ToString()));
                    }
                }
                else
                {
                    p_Comments = null;
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
                    obj.Comments = p_Comments;
                    return Result<BlogPost>.Ok(obj);
                }
                throw new NotImplementedException();
            }
        }
    }
}
