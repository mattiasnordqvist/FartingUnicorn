﻿// <auto-generated/>
using DotNetThoughts.Results;
using System.Text.Json;
using FartingUnicorn;

namespace FartingUnicorn.Tests;

// ClassName: BlogPost
// FullName: FartingUnicorn.Tests.SingleField.IntType.NonNullableNonOptional_Tests.BlogPost
// Namespace: FartingUnicorn.Tests
// Properties: 1
// HasCreateMapperAttribute: False
// ClassPath: SingleField, IntType, NonNullableNonOptional_Tests

// Property 0
// Name: Rating
// CompleteType: int
// IsArray: False
// IsObject: False
// IsNullable: False
// IsOption: False
// RawType: System.Int32


public partial class SingleField
{
    public partial class IntType
    {
        public partial class NonNullableNonOptional_Tests
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
                    var p_Rating = default(int);

                    List<IError> errors = new();
                    var isRatingPropertyDefined = jsonElement.TryGetProperty("Rating", out var jsonRatingProperty);
                    if (isRatingPropertyDefined)
                    {
                        if (jsonRatingProperty.ValueKind == JsonValueKind.Null)
                        {
                            errors.Add(new RequiredValueMissingError([.. path, "Rating"]));
                        }
                        else if (jsonRatingProperty.ValueKind == JsonValueKind.Number)
                        {
                            p_Rating = jsonRatingProperty.GetInt32();
                        }
                        else
                        {
                            errors.Add(new ValueHasWrongTypeError([.. path, "Rating"], "Number", jsonRatingProperty.ValueKind.ToString()));
                        }
                    }
                    else
                    {
                        errors.Add(new RequiredPropertyMissingError([.. path, "Rating"]));
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
                        obj.Rating = p_Rating;
                        return Result<BlogPost>.Ok(obj);
                    }
                    throw new NotImplementedException();
                }
            }
        }
    }
}
