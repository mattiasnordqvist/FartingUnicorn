﻿// <auto-generated/>
using DotNetThoughts.Results;
using System.Text.Json;
using FartingUnicorn;

namespace FartingUnicorn.Tests;

// ClassName: BlogPost
// FullName: FartingUnicorn.Tests.Arrays.PrimitiveIntArrays.BlogPost
// Namespace: FartingUnicorn.Tests
// Properties: 1
// HasCreateMapperAttribute: False
// ClassPath: Arrays, PrimitiveIntArrays

// Property 0
// Name: Ratings
// CompleteType: int[]
// IsArray: True
// IsObject: False
// IsNullable: False
// IsOption: False
// RawType: System.Int32[]
// ArrayElemCompleteType: int
// IsArrayElemArray: False
// IsArrayElemObject: False
// IsArrayElemOption: False
// ArrayElemRawType: System.Int32


public partial class Arrays
{
    public partial class PrimitiveIntArrays
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
                var p_Ratings = default(int[]);

                List<IError> errors = new();
                var isRatingsPropertyDefined = jsonElement.TryGetProperty("Ratings", out var jsonRatingsProperty);
                if (isRatingsPropertyDefined)
                {
                    if (jsonRatingsProperty.ValueKind == JsonValueKind.Null)
                    {
                        errors.Add(new RequiredValueMissingError([.. path, "Ratings"]));
                    }
                    else if (jsonRatingsProperty.ValueKind == JsonValueKind.Array)
                    {
                        var array = new int[jsonRatingsProperty.GetArrayLength()];
                        for(int i = 0; i < jsonRatingsProperty.GetArrayLength(); i++)
                        {
                            if(jsonRatingsProperty[i].ValueKind != JsonValueKind.Number)
                            {
                                errors.Add(new ValueHasWrongTypeError([.. path, "Ratings", i.ToString()], "Number", jsonRatingsProperty[i].ValueKind.ToString()));
                            }
                            else
                            {
                                array.SetValue(jsonRatingsProperty[i].GetInt32(), i);
                            }
                        }
                        p_Ratings = array;
                    }
                    else
                    {
                        errors.Add(new ValueHasWrongTypeError([.. path, "Ratings"], "Array", jsonRatingsProperty.ValueKind.ToString()));
                    }
                }
                else
                {
                    errors.Add(new RequiredPropertyMissingError([.. path, "Ratings"]));
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
                    obj.Ratings = p_Ratings;
                    return Result<BlogPost>.Ok(obj);
                }
                throw new NotImplementedException();
            }
        }
    }
}
