﻿// <auto-generated/>
using DotNetThoughts.Results;
using System.Text.Json;
using static FartingUnicorn.MapperOptions;

namespace FartingUnicorn.Tests;

// ClassName: Author
// FullName: FartingUnicorn.Tests.Objects.NullableOptional.Author
// Namespace: FartingUnicorn.Tests
// Properties: 2
// HasCreateMapperAttribute: False
// ClassPath: Objects, NullableOptional

// Property 0
// Name: Name
// TypeName: string
// IsArray: False
// IsObject: False
// IsNullable: False
// IsNullableValueType: False
// IsOption: False
// EffectiveType: System.String

// Property 1
// Name: Age
// TypeName: FartingUnicorn.Option<int>
// IsArray: False
// IsObject: False
// IsNullable: False
// IsNullableValueType: False
// IsOption: True
// EffectiveType: System.Int32


public partial class Objects
{
    public partial class NullableOptional
    {
        public partial class Author
        {
            // hello
            public static Result<Author> MapFromJson(JsonElement jsonElement, MapperOptions mapperOptions = null, string[] path = null)
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
                    return Result<Author>.Error(new ValueHasWrongTypeError(path, "Object", jsonElement.ValueKind.ToString()));
                }
                var obj = new Author();

                List<IError> errors = new();
                var isNamePropertyDefined = jsonElement.TryGetProperty("Name", out var jsonNameProperty);
                if (isNamePropertyDefined)
                {
                    // type = string, isOption = False, isNullable = False
                    if (jsonNameProperty.ValueKind == JsonValueKind.Null)
                    {
                        errors.Add(new RequiredValueMissingError([.. path, "Name"]));
                    }
                    else if (jsonNameProperty.ValueKind == JsonValueKind.String)
                    {
                        obj.Name = jsonNameProperty.GetString();
                    }
                    else
                    {
                        errors.Add(new ValueHasWrongTypeError([.. path, "Name"], "String", jsonNameProperty.ValueKind.ToString()));
                    }
                }
                else
                {
                    errors.Add(new RequiredPropertyMissingError([.. path, "Name"]));
                }
                var isAgePropertyDefined = jsonElement.TryGetProperty("Age", out var jsonAgeProperty);
                if (isAgePropertyDefined)
                {
                    // type = FartingUnicorn.Option<int>, isOption = True, isNullable = False
                    if (jsonAgeProperty.ValueKind == JsonValueKind.Null)
                    {
                        obj.Age = new None<System.Int32>();
                    }
                    else if (jsonAgeProperty.ValueKind == JsonValueKind.Number)
                    {
                        obj.Age = new Some<int>(jsonAgeProperty.GetInt32());
                    }
                    else
                    {
                        errors.Add(new ValueHasWrongTypeError([.. path, "Age"], "Number", jsonAgeProperty.ValueKind.ToString()));
                    }
                }
                else
                {
                    errors.Add(new RequiredPropertyMissingError([.. path, "Age"]));
                }
                if(errors.Any())
                {
                    return Result<Author>.Error(errors);
                }
                if(false)/*check if is option*/
                {
                }
                else
                {
                    return Result<Author>.Ok(obj);
                }
                throw new NotImplementedException();
            }
        }
    }
}
