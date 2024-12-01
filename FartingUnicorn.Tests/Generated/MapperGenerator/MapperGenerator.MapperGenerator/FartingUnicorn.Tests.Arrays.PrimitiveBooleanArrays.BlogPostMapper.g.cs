﻿// <auto-generated/>
using DotNetThoughts.Results;
using System.Text.Json;
using FartingUnicorn;

namespace FartingUnicorn.Tests;

// ClassName: BlogPost
// FullName: FartingUnicorn.Tests.Arrays.PrimitiveBooleanArrays.BlogPost
// Namespace: FartingUnicorn.Tests
// Properties: 1
// HasCreateMapperAttribute: False
// ClassPath: Arrays, PrimitiveBooleanArrays

// Property 0
// Name: Somethings
// CompleteType: bool[]
// IsArray: True
// IsObject: False
// IsNullable: False
// IsOption: False
// RawType: System.Boolean[]
// ArrayElemCompleteType: bool
// IsArrayElemArray: False
// IsArrayElemObject: False
// IsArrayElemOption: False
// ArrayElemRawType: System.Boolean


public partial class Arrays
{
    public partial class PrimitiveBooleanArrays
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
                var isSomethingsPropertyDefined = jsonElement.TryGetProperty("Somethings", out var jsonSomethingsProperty);
                if (isSomethingsPropertyDefined)
                {
                    if (jsonSomethingsProperty.ValueKind == JsonValueKind.Null)
                    {
                        errors.Add(new RequiredValueMissingError([.. path, "Somethings"]));
                    }
                    else if (jsonSomethingsProperty.ValueKind == JsonValueKind.Array)
                    {
                        var array = new bool[jsonSomethingsProperty.GetArrayLength()];
                        for(int i = 0; i < jsonSomethingsProperty.GetArrayLength(); i++)
                        {
                            if(jsonSomethingsProperty[i].ValueKind != JsonValueKind.True && jsonSomethingsProperty[i].ValueKind != JsonValueKind.False)
                            {
                                errors.Add(new ValueHasWrongTypeError([.. path, "Somethings", i.ToString()], "Boolean", jsonSomethingsProperty[i].ValueKind.ToString()));
                            }
                            else
                            {
                                array.SetValue(jsonSomethingsProperty[i].GetBoolean(), i);
                            }
                        }
                        obj.Somethings = array;
                    }
                    else
                    {
                        errors.Add(new ValueHasWrongTypeError([.. path, "Somethings"], "Array", jsonSomethingsProperty.ValueKind.ToString()));
                    }
                }
                else
                {
                    errors.Add(new RequiredPropertyMissingError([.. path, "Somethings"]));
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
