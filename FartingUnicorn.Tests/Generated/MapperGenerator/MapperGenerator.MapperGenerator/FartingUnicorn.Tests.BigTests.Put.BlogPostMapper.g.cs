﻿// <auto-generated/>
using DotNetThoughts.Results;
using System.Text.Json;
using static FartingUnicorn.MapperOptions;

namespace FartingUnicorn.Tests;

// ClassName: BlogPost
// FullName: FartingUnicorn.Tests.BigTests.Put.BlogPost
// Namespace: FartingUnicorn.Tests
// Properties: 6
// HasCreateMapperAttribute: False
// ClassPath: BigTests, Put

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

// Property 2
// Name: Category
// CompleteType: FartingUnicorn.Option<string>
// IsArray: False
// IsObject: False
// IsNullable: False
// IsOption: True
// RawType: System.String

// Property 3
// Name: Rating
// CompleteType: FartingUnicorn.Option<int>
// IsArray: False
// IsObject: False
// IsNullable: False
// IsOption: True
// RawType: System.Int32

// Property 4
// Name: Author
// CompleteType: FartingUnicorn.Tests.BigTests.Put.Author
// IsArray: False
// IsObject: True
// IsNullable: False
// IsOption: False
// RawType: FartingUnicorn.Tests.BigTests.Put.Author

// Property 5
// Name: Comments
// CompleteType: FartingUnicorn.Tests.BigTests.Put.Comment[]
// IsArray: True
// IsObject: False
// IsNullable: False
// IsOption: False
// RawType: FartingUnicorn.Tests.BigTests.Put.Comment[]
// ArrayElemCompleteType: FartingUnicorn.Tests.BigTests.Put.Comment
// IsArrayElemArray: False
// IsArrayElemObject: True
// IsArrayElemOption: False
// ArrayElemRawType: FartingUnicorn.Tests.BigTests.Put.Comment


public partial class BigTests
{
    public partial class Put
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
                        obj.IsDraft = jsonIsDraftProperty.GetBoolean();
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
                    errors.Add(new RequiredPropertyMissingError([.. path, "Category"]));
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
                    errors.Add(new RequiredPropertyMissingError([.. path, "Rating"]));
                }
                var isAuthorPropertyDefined = jsonElement.TryGetProperty("Author", out var jsonAuthorProperty);
                if (isAuthorPropertyDefined)
                {
                    if (jsonAuthorProperty.ValueKind == JsonValueKind.Null)
                    {
                        errors.Add(new RequiredValueMissingError([.. path, "Author"]));
                    }
                    else if (mapperOptions.TryGetConverter(typeof(FartingUnicorn.Tests.BigTests.Put.Author), out IConverter customConverter))
                    {
                        if (jsonAuthorProperty.ValueKind != customConverter.ExpectedJsonValueKind)
                        {
                            errors.Add(new ValueHasWrongTypeError([.. path, "Author"], customConverter.ExpectedJsonValueKind.ToString(), jsonAuthorProperty.ValueKind.ToString()));
                        }
                        else
                        {
                            var result = customConverter.Convert(typeof(FartingUnicorn.Tests.BigTests.Put.Author), jsonAuthorProperty, mapperOptions, [.. path, "Author"]);
                            if (result.Success)
                            {
                                obj.Author = result.Map(x => (FartingUnicorn.Tests.BigTests.Put.Author)x).Value;
                            }
                            else
                            {
                                errors.AddRange(result.Errors.Select(x => new MappingError([.. path, "Author"], x.Message)).ToArray());
                            }
                        }
                    }
                    else
                    {
                        if (jsonAuthorProperty.ValueKind == JsonValueKind.Object)
                        {
                            var result = FartingUnicorn.Tests.BigTests.Put.Author.MapFromJson(jsonAuthorProperty, mapperOptions, [.. path, "Author"]);
                            if (result.Success)
                            {
                                obj.Author = result.Value;
                            }
                            else
                            {
                                errors.AddRange(result.Errors.ToArray());
                            }
                        }
                        else
                        {
                            errors.Add(new ValueHasWrongTypeError([.. path, "Author"], "Object", jsonAuthorProperty.ValueKind.ToString()));
                        }
                    }
                }
                else
                {
                    errors.Add(new RequiredPropertyMissingError([.. path, "Author"]));
                }
                var isCommentsPropertyDefined = jsonElement.TryGetProperty("Comments", out var jsonCommentsProperty);
                if (isCommentsPropertyDefined)
                {
                    if (jsonCommentsProperty.ValueKind == JsonValueKind.Null)
                    {
                        errors.Add(new RequiredValueMissingError([.. path, "Comments"]));
                    }
                    else if (jsonCommentsProperty.ValueKind == JsonValueKind.Array)
                    {
                        var array = new FartingUnicorn.Tests.BigTests.Put.Comment[jsonCommentsProperty.GetArrayLength()];
                        for(int i = 0; i < jsonCommentsProperty.GetArrayLength(); i++)
                        {
                            var result = FartingUnicorn.Tests.BigTests.Put.Comment.MapFromJson(jsonCommentsProperty[i], mapperOptions, [.. path, "Comments", i.ToString()]);
                            if (result.Success)
                            {
                                array.SetValue(result.Value, i);
                            }
                            else
                            {
                                errors.AddRange(result.Errors.ToArray());
                            }
                        }
                        obj.Comments = array;
                    }
                    else
                    {
                        errors.Add(new ValueHasWrongTypeError([.. path, "Comments"], "Array", jsonCommentsProperty.ValueKind.ToString()));
                    }
                }
                else
                {
                    errors.Add(new RequiredPropertyMissingError([.. path, "Comments"]));
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