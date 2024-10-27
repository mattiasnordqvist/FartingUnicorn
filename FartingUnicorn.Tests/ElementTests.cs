using FluentAssertions;

using System.Text.Json;

using Xunit;

using static FartingUnicorn.Mapper;

namespace FartingUnicorn.Tests;

public class ElementTests
{
    public class String
    {
        [Fact]
        public void String_String_String()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                "Test"
                """);
            var result = Mapper.MapElement<string>(jsonElement);

            result.Success.Should().BeTrue();
            result.Value.Should().Be("Test");
        }

        [Fact]
        public void String_Null_Error()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                null
                """);
            var result = Mapper.MapElement<string>(jsonElement);
            result.Success.Should().BeFalse();
            result.Errors.Should().BeSingleError<RequiredValueMissingError>("$ must have a value");
        }

        [Fact]
        public void OptionString_String_SomeString()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                "Test"
                """);
            var result = Mapper.MapElement<Option<string>>(jsonElement);
            result.Success.Should().BeTrue();
            result.Value.Should().BeSome("Test");
        }

        [Fact]
        public void OptionString_Null_NoneString()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                null
                """);
            var result = Mapper.MapElement<Option<string>>(jsonElement);
            result.Success.Should().BeTrue();
            result.Value.Should().BeNone<string>();
        }

        [Fact]
        public void String_Number_Error()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                123456
                """);
            var result = Mapper.MapElement<string>(jsonElement);
            result.Success.Should().BeFalse();
            result.Errors.Should().BeSingleError<ValueHasWrongTypeError>("Value of $ has the wrong type. Expected String, got Number");
        }

        [Fact]
        public void OptionString_Number_Error()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                123456
                """);
            var result = Mapper.MapElement<Option<string>>(jsonElement);
            result.Success.Should().BeFalse();
            result.Errors.Should().BeSingleError<ValueHasWrongTypeError>("Value of $ has the wrong type. Expected String, got Number");
        }

        [Fact]
        public void String_Boolean_Error()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                true
                """);
            var result = Mapper.MapElement<string>(jsonElement);
            result.Success.Should().BeFalse();
            result.Errors.Should().BeSingleError<ValueHasWrongTypeError>("Value of $ has the wrong type. Expected String, got True");
        }

        [Fact]
        public void OptionString_Boolean_Error()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                true
                """);
            var result = Mapper.MapElement<Option<string>>(jsonElement);
            result.Success.Should().BeFalse();
            result.Errors.Should().BeSingleError<ValueHasWrongTypeError>("Value of $ has the wrong type. Expected String, got True");
        }

        [Fact]
        public void String_Object_Error()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {}
                """);
            var result = Mapper.MapElement<string>(jsonElement);
            result.Success.Should().BeFalse();
            result.Errors.Should().BeSingleError<ValueHasWrongTypeError>("Value of $ has the wrong type. Expected String, got Object");
        }

        [Fact]
        public void OptionString_Object_Error()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {}
                """);
            var result = Mapper.MapElement<Option<string>>(jsonElement);
            result.Success.Should().BeFalse();
            result.Errors.Should().BeSingleError<ValueHasWrongTypeError>("Value of $ has the wrong type. Expected String, got Object");
        }

        [Fact]
        public void String_Array_Error()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                []
                """);
            var result = Mapper.MapElement<string>(jsonElement);
            result.Success.Should().BeFalse();
            result.Errors.Should().BeSingleError<ValueHasWrongTypeError>("Value of $ has the wrong type. Expected String, got Array");
        }

        [Fact]
        public void OptionString_Array_Error()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                []
                """);
            var result = Mapper.MapElement<Option<string>>(jsonElement);
            result.Success.Should().BeFalse();
            result.Errors.Should().BeSingleError<ValueHasWrongTypeError>("Value of $ has the wrong type. Expected String, got Array");
        }
    }

    public class Number
    {
        [Fact]
        public void Number_Number_Number()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                123456
                """);
            var result = Mapper.MapElement<int>(jsonElement);

            result.Success.Should().BeTrue();
            result.Value.Should().Be(123456);
        }

        [Fact]
        public void Number_Null_Error()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                null
                """);
            var result = Mapper.MapElement<int>(jsonElement);
            result.Success.Should().BeFalse();
            result.Errors.Should().BeSingleError<RequiredValueMissingError>("$ must have a value");
        }
        [Fact]
        public void OptionNumber_Number_SomeNumber()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                123456
                """);
            var result = Mapper.MapElement<Option<int>>(jsonElement);
            result.Success.Should().BeTrue();
            result.Value.Should().BeSome(123456);
        }
        [Fact]
        public void OptionNumber_Null_NoneNumber()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                null
                """);
            var result = Mapper.MapElement<Option<int>>(jsonElement);
            result.Success.Should().BeTrue();
            result.Value.Should().BeNone<int>();
        }
        [Fact]
        public void Number_String_Error()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                "Test"
                """);
            var result = Mapper.MapElement<int>(jsonElement);
            result.Success.Should().BeFalse();
            result.Errors.Should().BeSingleError<ValueHasWrongTypeError>("Value of $ has the wrong type. Expected Number, got String");
        }
    }

    public class Boolean
    {
        [Fact]
        public void Boolean_True_Boolean()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                true
                """);
            var result = Mapper.MapElement<bool>(jsonElement);

            result.Success.Should().BeTrue();
            result.Value.Should().Be(true);
        }

        [Fact]
        public void Boolean_False_Boolean()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                false
                """);
            var result = Mapper.MapElement<bool>(jsonElement);

            result.Success.Should().BeTrue();
            result.Value.Should().Be(false);
        }

        [Fact]
        public void Boolean_Null_Error()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                null
                """);
            var result = Mapper.MapElement<bool>(jsonElement);
            result.Success.Should().BeFalse();
            result.Errors.Should().BeSingleError<RequiredValueMissingError>("$ must have a value");
        }

        [Fact]
        public void OptionBoolean_True_SomeBoolean()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                true
                """);
            var result = Mapper.MapElement<Option<bool>>(jsonElement);
            result.Success.Should().BeTrue();
            result.Value.Should().BeSome(true);
        }

        [Fact]
        public void OptionBoolean_Null_NoneBoolean()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                null
                """);
            var result = Mapper.MapElement<Option<bool>>(jsonElement);
            result.Success.Should().BeTrue();
            result.Value.Should().BeNone<bool>();
        }

        [Fact]
        public void Boolean_String_Error()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                "Test"
                """);
            var result = Mapper.MapElement<bool>(jsonElement);
            result.Success.Should().BeFalse();
            result.Errors.Should().BeSingleError<ValueHasWrongTypeError>("Value of $ has the wrong type. Expected Boolean, got String");
        }
    }

    public class Object
    {
        public class BlogPost { }

        [Fact]
        public void Object_Object_Object()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {}
                """);
            var result = Mapper.MapElement<BlogPost>(jsonElement);
            result.Success.Should().BeTrue();
            result.Value.Should().BeOfType<BlogPost>();
        }

        [Fact]
        public void Object_Null_Error()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                null
                """);
            var result = Mapper.MapElement<BlogPost>(jsonElement);
            result.Success.Should().BeFalse();
            result.Errors.Should().BeSingleError<RequiredValueMissingError>("$ must have a value");
        }

        [Fact]
        public void OptionObject_Object_SomeObject()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {}
                """);
            var result = Mapper.MapElement<Option<BlogPost>>(jsonElement);
            result.Success.Should().BeTrue();
            result.Value.Should().BeOfType<Some<BlogPost>>();
        }

        [Fact]
        public void OptionObject_Null_NoneObject()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                null
                """);
            var result = Mapper.MapElement<Option<BlogPost>>(jsonElement);
            result.Success.Should().BeTrue();
            result.Value.Should().BeOfType<None<BlogPost>>();
        }

        [Fact]
        public void Object_String_Error()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                "Test"
                """);
            var result = Mapper.MapElement<BlogPost>(jsonElement);
            result.Success.Should().BeFalse();
            result.Errors.Should().BeSingleError<ValueHasWrongTypeError>("Value of $ has the wrong type. Expected Object, got String");
        }

        public class WithProperties
        {
            public class BlogPost
            {
                public string Title { get; set; }
            }

            [Fact]
            public void ObjectString_ObjectString_Object()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                        "Title": "Farting Llamacorns"
                    }
                    """);
                var result = Mapper.MapElement<BlogPost>(jsonElement);
                result.Success.Should().BeTrue();
                result.Value.Should().BeOfType<BlogPost>();
                var blogPost = (BlogPost)result.Value!;
                blogPost.Title.Should().Be("Farting Llamacorns");
            }

            [Fact]
            public void ObjectString_ObjectNull_Error()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                        "Title": null
                    }
                    """);
                var result = Mapper.MapElement<BlogPost>(jsonElement);
                result.Success.Should().BeFalse();
                result.Errors.Should().BeSingleError<RequiredValueMissingError>("$.Title must have a value");
            }

            [Fact]
            public void Object_Object_Null_Error()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                        "Title": null
                    }
                    """);
                var result = Mapper.MapElement<BlogPost>(jsonElement);
                result.Success.Should().BeFalse();
                result.Errors.Should().BeSingleError<RequiredValueMissingError>("$.Title must have a value");
            }

            public class WithObjectProperty
            {
                public class BlogPost
                {
                    public string Title { get; set; }
                    public Author Author { get; set; }
                }
                public class Author
                {
                    public string Name { get; set; }
                }

                [Fact]
                public void ObjectObjectString_ObjectObjectString_Object()
                {
                    var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                        {
                            "Title": "Farting Llamacorns",
                            "Author": {
                                "Name": "John Doe"
                            }
                        }
                        """);
                    var result = Mapper.MapElement<BlogPost>(jsonElement);
                    result.Success.Should().BeTrue();
                    result.Value.Should().BeOfType<BlogPost>();
                    var blogPost = (BlogPost)result.Value!;
                    blogPost.Title.Should().Be("Farting Llamacorns");
                    blogPost.Author.Should().NotBeNull();
                    blogPost.Author.Name.Should().Be("John Doe");
                }

                [Fact]
                public void ObjectObjectString_ObjectNull_Error()
                {
                    var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                        {
                            "Title": "Farting Llamacorns",
                            "Author": null
                        }
                        """);
                    var result = Mapper.MapElement<BlogPost>(jsonElement);
                    result.Success.Should().BeFalse();
                    result.Errors.Should().BeSingleError<RequiredValueMissingError>("$.Author must have a value");
                }

                [Fact]
                public void ObjectObjectString_ObjectObjectNull_Error()
                {
                    var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                        {
                            "Title": "Farting Llamacorns",
                            "Author": {
                                "Name": null
                            }
                        }
                        """);
                    var result = Mapper.MapElement<BlogPost>(jsonElement);
                    result.Success.Should().BeFalse();
                    result.Errors.Should().BeSingleError<RequiredValueMissingError>("$.Author.Name must have a value");
                }
            }
        }
    }

    public class Array
    {
        public class BlogPost { }

        [Fact]
        public void Array_EmptyArray_EmptyArray()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                []
                """);
            var result = Mapper.MapElement<BlogPost[]>(jsonElement);
            result.Success.Should().BeTrue();
            result.Value.Should().BeOfType<BlogPost[]>();
        }
        [Fact]
        public void Array_Null_Error()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                null
                """);
            var result = Mapper.MapElement<BlogPost[]>(jsonElement);
            result.Success.Should().BeFalse();
            result.Errors.Should().BeSingleError<RequiredValueMissingError>("$ must have a value");
        }

        [Fact]
        public void OptionArray_EmptyArray_SomeEmptyArray()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                []
                """);
            var result = Mapper.MapElement<Option<BlogPost[]>>(jsonElement);
            result.Success.Should().BeTrue();
            result.Value.Should().BeOfType<Some<BlogPost[]>>();
            var some = (Some<BlogPost[]>)result.Value!;
            some.Value.Should().BeEmpty();
        }

        [Fact]
        public void OptionArray_Null_NoneArray()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                null
                """);
            var result = Mapper.MapElement<Option<BlogPost[]>>(jsonElement);
            result.Success.Should().BeTrue();
            result.Value.Should().BeOfType<None<BlogPost[]>>();
        }

        [Fact]
        public void StringArray_StringArray_StringArray()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    ["Test1", "Test2", "Test3"]
                    """);
            var result = Mapper.MapElement<string[]>(jsonElement);
            result.Success.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(new[] { "Test1", "Test2", "Test3" });
        }

        [Fact]
        public void StringArray_StringArrayWithNull_Error()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    ["Test1", null, "Test3"]
                    """);
            var result = Mapper.MapElement<string[]>(jsonElement);
            result.Success.Should().BeFalse();
            result.Errors.Should().BeSingleError<RequiredValueMissingError>("$.1 must have a value");
        }

        [Fact]
        public void OptionStringArray_StringArrayWithNull_SomeNoneSomeStringArray()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    ["Test1", null, "Test3"]
                    """);
            var result = Mapper.MapElement<Option<string>[]>(jsonElement);
            result.Success.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(new Option<string>[]
            {
                new Some<string>("Test1"),
                new None<string>(),
                new Some<string>("Test3")
            }, config => config.ComparingByValue<Option<string>>());

        }
    }
    public class ObjectWithArrayOfStrings
    {
        public class BlogPost
        {
            public string[] Tags { get; set; }
        }

        [Fact]
        public void ObjectStringArray_ObjectStringArray_Object()
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Tags": ["Tag1", "Tag2", "Tag3"]
                }
                """);
            var result = Mapper.MapElement<BlogPost>(jsonElement);
            result.Success.Should().BeTrue();
            result.Value.Should().BeOfType<BlogPost>();
            var blogPost = (BlogPost)result.Value!;
            blogPost.Tags.Should().BeEquivalentTo(new[] { "Tag1", "Tag2", "Tag3" });
        }
    }
}
