﻿// <auto-generated/>
using DotNetThoughts.Results;
using System.Text.Json;
using static FartingUnicorn.MapperOptions;

namespace FartingUnicorn.Tests;

// ClassName: BlogPost
// FullName: FartingUnicorn.Tests.Objects.NullableOptional.BlogPost
// Namespace: FartingUnicorn.Tests
// Properties: 1
// HasCreateMapperAttribute: False
// ClassPath: Objects, NullableOptional

// Property 0
// Name: Author
// TypeName: FartingUnicorn.Option<FartingUnicorn.Tests.Objects.NullableOptional.Author>?
// IsArray: False
// IsObject: True
// IsNullable: True
// IsNullableValueType: False
// IsOption: True
// EffectiveType: FartingUnicorn.Tests.Objects.NullableOptional.Author


public partial class Objects
{
    public partial class NullableOptional
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
                var isAuthorPropertyDefined = jsonElement.TryGetProperty("Author", out var jsonAuthorProperty);
                if (isAuthorPropertyDefined)
                {
                    // type = FartingUnicorn.Option<FartingUnicorn.Tests.Objects.NullableOptional.Author>?, isOption = True, isNullable = True
                    if (jsonAuthorProperty.ValueKind == JsonValueKind.Null)
                    {
                        obj.Author = new None<FartingUnicorn.Tests.Objects.NullableOptional.Author>();
                    }
                    else if (mapperOptions.TryGetConverter(typeof(FartingUnicorn.Tests.Objects.NullableOptional.Author), out IConverter customConverter))
                    {
                        if (jsonAuthorProperty.ValueKind != customConverter.ExpectedJsonValueKind)
                        {
                            errors.Add(new ValueHasWrongTypeError([.. path, "Author"], customConverter.ExpectedJsonValueKind.ToString(), jsonAuthorProperty.ValueKind.ToString()));
                        }
                        else
                        {
                            var result = customConverter.Convert(typeof(FartingUnicorn.Tests.Objects.NullableOptional.Author), jsonAuthorProperty, mapperOptions, [.. path, "Author"]);
                            if (result.Success)
                            {
                                obj.Author = new Some<FartingUnicorn.Tests.Objects.NullableOptional.Author>(result.Map(x => (FartingUnicorn.Tests.Objects.NullableOptional.Author)x).Value);
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
                            var result = FartingUnicorn.Tests.Objects.NullableOptional.Author.MapFromJson(jsonAuthorProperty, mapperOptions, [.. path, "Author"]);
                            if (result.Success)
                            {
                                obj.Author = new Some<FartingUnicorn.Tests.Objects.NullableOptional.Author>(result.Value!);
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
                    obj.Author = null;
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
