﻿// <auto-generated/>
using DotNetThoughts.Results;
using System.Text.Json;
using static FartingUnicorn.MapperOptions;

namespace FartingUnicorn.Tests;

// ClassName: Id
// FullName: FartingUnicorn.Tests.Converters.Id
// Namespace: FartingUnicorn.Tests
// Properties: 1
// HasCreateMapperAttribute: False
// ClassPath: Converters

// Property 0
// Name: Value
// TypeName: long
// IsArray: False
// IsReferenceType: False
// IsNullable: False
// IsOption: False
// EffectiveType: System.Int64


public partial class Converters
{
    public partial class Id
    {
        // hello
        public static Result<Id> MapFromJson(JsonElement jsonElement, MapperOptions mapperOptions = null, string[] path = null)
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
                return Result<Id>.Error(new ValueHasWrongTypeError(path, "Object", jsonElement.ValueKind.ToString()));
            }
            var obj = new Id();

            List<IError> errors = new();
            var isValuePropertyDefined = jsonElement.TryGetProperty("Value", out var jsonValueProperty);
            if (isValuePropertyDefined)
            {
                // type = long, isOption = False, isNullable = False
                if (jsonValueProperty.ValueKind == JsonValueKind.Null)
                {
                    errors.Add(new RequiredValueMissingError([.. path, "Value"]));
                }
                else if (mapperOptions.TryGetConverter(typeof(System.Int64), out IConverter customConverter))
                {
                    if (jsonValueProperty.ValueKind != customConverter.ExpectedJsonValueKind)
                    {
                        errors.Add(new ValueHasWrongTypeError([.. path, "Value"], customConverter.ExpectedJsonValueKind.ToString(), jsonValueProperty.ValueKind.ToString()));
                    }
                    else
                    {
                        var result = customConverter.Convert(typeof(System.Int64), jsonValueProperty, mapperOptions, [.. path, "Value"]);
                        if (result.Success)
                        {
                            obj.Value = result.Map(x => (System.Int64)x).Value;
                        }
                        else
                        {
                            errors.AddRange(result.Errors.Select(x => new MappingError([.. path, "Value"], x.Message)).ToArray());
                        }
                    }
                }
            }
            else
            {
                errors.Add(new RequiredPropertyMissingError([.. path, "Value"]));
            }
            if(errors.Any())
            {
                return Result<Id>.Error(errors);
            }
            if(false)/*check if is option*/
            {
            }
            else
            {
                return Result<Id>.Ok(obj);
            }
            throw new NotImplementedException();
        }
    }
}
