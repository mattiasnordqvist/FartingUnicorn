﻿// <auto-generated/>
using DotNetThoughts.Results;
using System.Text.Json;
using static FartingUnicorn.MapperOptions;

namespace FartingUnicorn.Tests;

// ClassName: BlogPost
// FullName: FartingUnicorn.Tests.Arrays.PrimitiveArrays.BlogPost
// Namespace: FartingUnicorn.Tests
// Properties: 1
// HasCreateMapperAttribute: False
// ClassPath: Arrays, PrimitiveArrays

// Property 0
// Name: Categories
// CompleteType: string[]
// IsArray: True
// IsObject: False
// IsNullable: False
// IsOption: False
// RawType: System.String[]
// ArrayElemCompleteType: string
// IsArrayElemArray: False
// IsArrayElemObject: False
// IsArrayElemOption: False
// ArrayElemRawType: System.String


public partial class Arrays
{
    public partial class PrimitiveArrays
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
                var isCategoriesPropertyDefined = jsonElement.TryGetProperty("Categories", out var jsonCategoriesProperty);
                if (isCategoriesPropertyDefined)
                {
                    if (jsonCategoriesProperty.ValueKind == JsonValueKind.Null)
                    {
                        errors.Add(new RequiredValueMissingError([.. path, "Categories"]));
                    }
                    else if (jsonCategoriesProperty.ValueKind == JsonValueKind.Array)
                    {
                        var array = new string[jsonCategoriesProperty.GetArrayLength()];
                        for(int i = 0; i < jsonCategoriesProperty.GetArrayLength(); i++)
                        {
                            if(jsonCategoriesProperty[i].ValueKind != JsonValueKind.String)
                            {
                                errors.Add(new ValueHasWrongTypeError([.. path, "Categories"], "String", jsonCategoriesProperty[i].ValueKind.ToString()));
                            }
                            else
                            {
                                array.SetValue(jsonCategoriesProperty[i].GetString(), i);
                            }
                        }
                        obj.Categories = array;
                    }
                    else
                    {
                        errors.Add(new ValueHasWrongTypeError([.. path, "Categories"], "Array", jsonCategoriesProperty.ValueKind.ToString()));
                    }
                }
                else
                {
                    errors.Add(new RequiredPropertyMissingError([.. path, "Categories"]));
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
