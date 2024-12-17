﻿// <auto-generated/>
using DotNetThoughts.Results;
using System.Text.Json;
using FartingUnicorn;

namespace FartingUnicorn.Tests;

// ClassName: Comment
// FullName: FartingUnicorn.Tests.Arrays.Nullable.Comment
// Namespace: FartingUnicorn.Tests
// Properties: 3
// HasCreateMapperAttribute: False
// ClassPath: Arrays, Nullable

// Property 0
// Name: Text
// CompleteType: string
// IsArray: False
// IsObject: False
// IsNullable: False
// IsOption: False
// RawType: System.String

// Property 1
// Name: Upvotes
// CompleteType: int
// IsArray: False
// IsObject: False
// IsNullable: False
// IsOption: False
// RawType: System.Int32

// Property 2
// Name: Contact
// CompleteType: FartingUnicorn.Option<string>
// IsArray: False
// IsObject: False
// IsNullable: False
// IsOption: True
// RawType: System.String


public partial class Arrays
{
    public partial class Nullable
    {
        public partial class Comment
        {
            public static Result<Comment> MapFromJson(JsonElement jsonElement, MapperOptions mapperOptions = null, string[] path = null)
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
                    return Result<Comment>.Error(new ValueHasWrongTypeError(path, "Object", jsonElement.ValueKind.ToString()));
                }
                var p_Text = default(string);
                var p_Upvotes = default(int);
                var p_Contact = default(FartingUnicorn.Option<string>);

                List<IError> errors = new();
                var isTextPropertyDefined = jsonElement.TryGetProperty("Text", out var jsonTextProperty);
                if (isTextPropertyDefined)
                {
                    if (jsonTextProperty.ValueKind == JsonValueKind.Null)
                    {
                        errors.Add(new RequiredValueMissingError([.. path, "Text"]));
                    }
                    else if (jsonTextProperty.ValueKind == JsonValueKind.String)
                    {
                        p_Text = jsonTextProperty.GetString();
                    }
                    else
                    {
                        errors.Add(new ValueHasWrongTypeError([.. path, "Text"], "String", jsonTextProperty.ValueKind.ToString()));
                    }
                }
                else
                {
                    errors.Add(new RequiredPropertyMissingError([.. path, "Text"]));
                }
                var isUpvotesPropertyDefined = jsonElement.TryGetProperty("Upvotes", out var jsonUpvotesProperty);
                if (isUpvotesPropertyDefined)
                {
                    if (jsonUpvotesProperty.ValueKind == JsonValueKind.Null)
                    {
                        errors.Add(new RequiredValueMissingError([.. path, "Upvotes"]));
                    }
                    else if (jsonUpvotesProperty.ValueKind == JsonValueKind.Number)
                    {
                        p_Upvotes = jsonUpvotesProperty.GetInt32();
                    }
                    else
                    {
                        errors.Add(new ValueHasWrongTypeError([.. path, "Upvotes"], "Number", jsonUpvotesProperty.ValueKind.ToString()));
                    }
                }
                else
                {
                    errors.Add(new RequiredPropertyMissingError([.. path, "Upvotes"]));
                }
                var isContactPropertyDefined = jsonElement.TryGetProperty("Contact", out var jsonContactProperty);
                if (isContactPropertyDefined)
                {
                    if (jsonContactProperty.ValueKind == JsonValueKind.Null)
                    {
                        p_Contact = new None<System.String>();
                    }
                    else if (jsonContactProperty.ValueKind == JsonValueKind.String)
                    {
                        p_Contact = new Some<string>(jsonContactProperty.GetString());
                    }
                    else
                    {
                        errors.Add(new ValueHasWrongTypeError([.. path, "Contact"], "String", jsonContactProperty.ValueKind.ToString()));
                    }
                }
                else
                {
                    errors.Add(new RequiredPropertyMissingError([.. path, "Contact"]));
                }
                if(errors.Any())
                {
                    return Result<Comment>.Error(errors);
                }
                if(false)/*check if is option*/
                {
                }
                else
                {
                    var obj = new Comment();
                    obj.Text = p_Text;
                    obj.Upvotes = p_Upvotes;
                    obj.Contact = p_Contact;
                    return Result<Comment>.Ok(obj);
                }
                throw new NotImplementedException();
            }
        }
    }
}
