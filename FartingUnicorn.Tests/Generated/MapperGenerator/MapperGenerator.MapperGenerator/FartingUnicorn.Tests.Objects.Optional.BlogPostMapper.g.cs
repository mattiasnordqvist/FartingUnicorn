﻿// <auto-generated/>
using DotNetThoughts.Results;
using System.Text.Json;
using static FartingUnicorn.MapperOptions;

namespace FartingUnicorn.Tests;

// ClassName: BlogPost
// FullName: FartingUnicorn.Tests.Objects.Optional.BlogPost
// Namespace: FartingUnicorn.Tests
// Properties: 5
// HasCreateMapperAttribute: False
// ClassPath: Objects, Optional

// Property 0
// Name: Title
// TypeName: string
// IsArray: False
// IsReferenceType: True
// IsNullable: False
// IsOption: False
// EffectiveType: System.String

// Property 1
// Name: IsDraft
// TypeName: bool
// IsArray: False
// IsReferenceType: False
// IsNullable: False
// IsOption: False
// EffectiveType: System.Boolean

// Property 2
// Name: Category
// TypeName: FartingUnicorn.Option<string>
// IsArray: False
// IsReferenceType: False
// IsNullable: False
// IsOption: True
// EffectiveType: System.String

// Property 3
// Name: Rating
// TypeName: FartingUnicorn.Option<int>
// IsArray: False
// IsReferenceType: False
// IsNullable: False
// IsOption: True
// EffectiveType: System.Int32

// Property 4
// Name: Author
// TypeName: FartingUnicorn.Option<FartingUnicorn.Tests.Objects.Optional.Author>
// IsArray: False
// IsReferenceType: False
// IsNullable: False
// IsOption: True
// EffectiveType: FartingUnicorn.Tests.Objects.Optional.Author


public partial class Objects
{
    public partial class Optional
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
                var isIsDraftPropertyDefined = jsonElement.TryGetProperty("IsDraft", out var jsonIsDraftProperty);
                if (isIsDraftPropertyDefined)
                {
                    // type = bool, isOption = False, isNullable = False
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
                    // type = FartingUnicorn.Option<string>, isOption = True, isNullable = False
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
                    // type = FartingUnicorn.Option<int>, isOption = True, isNullable = False
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
                    // type = FartingUnicorn.Option<FartingUnicorn.Tests.Objects.Optional.Author>, isOption = True, isNullable = False
                    if (jsonAuthorProperty.ValueKind == JsonValueKind.Null)
                    {
                        obj.Author = new None<FartingUnicorn.Tests.Objects.Optional.Author>();
                    }
                    else if (mapperOptions.TryGetConverter(typeof(FartingUnicorn.Tests.Objects.Optional.Author), out IConverter customConverter))
                    {
                        if (jsonAuthorProperty.ValueKind != customConverter.ExpectedJsonValueKind)
                        {
                            errors.Add(new ValueHasWrongTypeError([.. path, "Author"], customConverter.ExpectedJsonValueKind.ToString(), jsonAuthorProperty.ValueKind.ToString()));
                        }
                        else
                        {
                            var result = customConverter.Convert(typeof(FartingUnicorn.Tests.Objects.Optional.Author), jsonAuthorProperty, mapperOptions, [.. path, "Author"]);
                            if (result.Success)
                            {
                                obj.Author = new Some<FartingUnicorn.Tests.Objects.Optional.Author>(result.Map(x => (FartingUnicorn.Tests.Objects.Optional.Author)x).Value);
                            }
                            else
                            {
                                errors.AddRange(result.Errors.Select(x => new MappingError([.. path, "Author"], x.Message)).ToArray());
                            }
                        }
                    }
                }
                else
                {
                    errors.Add(new RequiredPropertyMissingError([.. path, "Author"]));
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
