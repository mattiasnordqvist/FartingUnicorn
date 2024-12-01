﻿// <auto-generated/>
using DotNetThoughts.Results;
using System.Text.Json;
using static FartingUnicorn.MapperOptions;

namespace FartingUnicorn.Tests;

// ClassName: BlogPost
// FullName: FartingUnicorn.Tests.SingleField.EnumType.NonNullableNonOptional_Tests.BlogPost
// Namespace: FartingUnicorn.Tests
// Properties: 1
// HasCreateMapperAttribute: False
// ClassPath: SingleField, EnumType, NonNullableNonOptional_Tests

// Property 0
// Name: Status
// TypeName: FartingUnicorn.Tests.SingleField.EnumType.NonNullableNonOptional_Tests.BlogPost.BlogPostStatus
// IsArray: False
// IsReferenceType: False
// IsNullable: False
// IsOption: False
// EffectiveType: FartingUnicorn.Tests.SingleField.EnumType.NonNullableNonOptional_Tests.BlogPost.BlogPostStatus


public partial class SingleField
{
    public partial class EnumType
    {
        public partial class NonNullableNonOptional_Tests
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
                    var isStatusPropertyDefined = jsonElement.TryGetProperty("Status", out var jsonStatusProperty);
                    if (isStatusPropertyDefined)
                    {
                        // type = FartingUnicorn.Tests.SingleField.EnumType.NonNullableNonOptional_Tests.BlogPost.BlogPostStatus, isOption = False, isNullable = False
                        if (jsonStatusProperty.ValueKind == JsonValueKind.Null)
                        {
                            errors.Add(new RequiredValueMissingError([.. path, "Status"]));
                        }
                        else if (mapperOptions.TryGetConverter(typeof(FartingUnicorn.Tests.SingleField.EnumType.NonNullableNonOptional_Tests.BlogPost.BlogPostStatus), out IConverter customConverter))
                        {
                            if (jsonStatusProperty.ValueKind != customConverter.ExpectedJsonValueKind)
                            {
                                errors.Add(new ValueHasWrongTypeError([.. path, "Status"], customConverter.ExpectedJsonValueKind.ToString(), jsonStatusProperty.ValueKind.ToString()));
                            }
                            else
                            {
                                var result = customConverter.Convert(typeof(FartingUnicorn.Tests.SingleField.EnumType.NonNullableNonOptional_Tests.BlogPost.BlogPostStatus), jsonStatusProperty, mapperOptions, [.. path, "Status"]);
                                if (result.Success)
                                {
                                    obj.Status = result.Map(x => (FartingUnicorn.Tests.SingleField.EnumType.NonNullableNonOptional_Tests.BlogPost.BlogPostStatus)x).Value;
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
                        return Result<BlogPost>.Ok(obj);
                    }
                    throw new NotImplementedException();
                }
            }
        }
    }
}