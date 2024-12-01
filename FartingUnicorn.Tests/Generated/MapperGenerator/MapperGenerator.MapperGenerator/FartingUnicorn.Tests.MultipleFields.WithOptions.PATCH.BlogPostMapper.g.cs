﻿// <auto-generated/>
using DotNetThoughts.Results;
using System.Text.Json;
using FartingUnicorn;

namespace FartingUnicorn.Tests;

// ClassName: BlogPost
// FullName: FartingUnicorn.Tests.MultipleFields.WithOptions.PATCH.BlogPost
// Namespace: FartingUnicorn.Tests
// Properties: 2
// HasCreateMapperAttribute: False
// ClassPath: MultipleFields, WithOptions, PATCH

// Property 0
// Name: Category
// CompleteType: FartingUnicorn.Option<string>?
// IsArray: False
// IsObject: False
// IsNullable: True
// IsOption: True
// RawType: System.String

// Property 1
// Name: Rating
// CompleteType: FartingUnicorn.Option<int>?
// IsArray: False
// IsObject: False
// IsNullable: True
// IsOption: True
// RawType: System.Int32


public partial class MultipleFields
{
    public partial class WithOptions
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
                    var isCategoryPropertyDefined = jsonElement.TryGetProperty("Category", out var jsonCategoryProperty);
                    if (isCategoryPropertyDefined)
                    {
                        if (jsonCategoryProperty.ValueKind == JsonValueKind.Null)
                        {
                            obj.Category = new None<System.String>();
                        }
                        else if (jsonCategoryProperty.ValueKind == JsonValueKind.String)
                        {
                            obj.Category = new Some<string>(jsonCategoryProperty.GetString());
                        }
                        else
                        {
                            errors.Add(new ValueHasWrongTypeError([.. path, "Category"], "String", jsonCategoryProperty.ValueKind.ToString()));
                        }
                    }
                    else
                    {
                        obj.Category = null;
                    }
                    var isRatingPropertyDefined = jsonElement.TryGetProperty("Rating", out var jsonRatingProperty);
                    if (isRatingPropertyDefined)
                    {
                        if (jsonRatingProperty.ValueKind == JsonValueKind.Null)
                        {
                            obj.Rating = new None<System.Int32>();
                        }
                        else if (jsonRatingProperty.ValueKind == JsonValueKind.Number)
                        {
                            obj.Rating = new Some<int>(jsonRatingProperty.GetInt32());
                        }
                        else
                        {
                            errors.Add(new ValueHasWrongTypeError([.. path, "Rating"], "Number", jsonRatingProperty.ValueKind.ToString()));
                        }
                    }
                    else
                    {
                        obj.Rating = null;
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
