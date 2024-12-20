﻿using DotNetThoughts.FartingUnicorn;
using DotNetThoughts.Results;

using FluentAssertions;

using System.Text.Json;

using Xunit;

using static FartingUnicorn.Mapper;
namespace FartingUnicorn.Tests;

public partial class SingleField
{
    public partial class StringType
    {
        public partial class NonNullableNonOptional_Tests
        {
            public static IEnumerable<object[]> GetMappers =>
                [
                    [(Func<JsonElement, Result<BlogPost>>)(x => Map<BlogPost>(x, null, null))],
                    [(Func<JsonElement, Result<BlogPost>>)(x => BlogPost.MapFromJson(x, null, null))]

                ];

            [CreateMapper]
            public partial class BlogPost
            {
                /// <summary>
                /// Field must exist
                /// Value cannot be null
                /// </summary>
                public string Title { get; set; }
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void ValidSingleField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "Title": "Farting Unicorns"
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.True(blogPost.Success);
                Assert.Equal("Farting Unicorns", blogPost.Value.Title);
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void MissingNonNullableField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.False(blogPost.Success);
                blogPost.Errors.Should().ContainSingle();
                blogPost.Errors.Single().Should().BeOfType<RequiredPropertyMissingError>();
                blogPost.Errors.Single().Message.Should().Be("$.Title is required");
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void NulledNonNullableField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "Title": null
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.False(blogPost.Success);
                blogPost.Errors.Should().ContainSingle();
                blogPost.Errors.Single().Should().BeOfType<RequiredValueMissingError>();
                blogPost.Errors.Single().Message.Should().Be("$.Title must have a value");
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void InvalidFieldType(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "Title": 123456
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.False(blogPost.Success);
                blogPost.Errors.Should().ContainSingle();
                blogPost.Errors.Single().Should().BeOfType<ValueHasWrongTypeError>();
                blogPost.Errors.Single().Message.Should().Be("Value of $.Title has the wrong type. Expected String, got Number");
            }
        }

        public partial class NullableNonOptional_Tests
        {
            public static IEnumerable<object[]> GetMappers =>
            [
                [(Func<JsonElement, Result<BlogPost>>)(x => Map<BlogPost>(x, null, null))],
                [(Func<JsonElement, Result<BlogPost>>)(x => BlogPost.MapFromJson(x, null))]
            ];

            [CreateMapper]
            public partial class BlogPost
            {
                /// <summary>
                /// Field can be missing
                /// Value cannot be null
                /// </summary>
                public string? Title { get; set; }
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void ValidSingleField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "Title": "Farting Unicorns"
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.True(blogPost.Success);
                Assert.Equal("Farting Unicorns", blogPost.Value.Title);
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void MissingNullableField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.True(blogPost.Success);
                Assert.Null(blogPost.Value.Title);
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void NulledNullableField(Func<JsonElement, Result<BlogPost>> map)
            {
                // Might seem counterintuitive, but remember, we said that 
                // clr-null should reflect a missing field. In this case, the field exists, but does not have a value.
                // This would be equivalent to None in an Option type.
                // Therefor, this is not valid json for a nullable field. 
                // If the field exists, it must have a value!
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
            {
              "Title": null
            }
            """);
                var blogPost = map(jsonElement);

                Assert.False(blogPost.Success);
                blogPost.Errors.Should().ContainSingle();
                blogPost.Errors.Single().Should().BeOfType<RequiredValueMissingError>();
                blogPost.Errors.Single().Message.Should().Be("$.Title must have a value");
            }
        }

        public partial class NonNullableOptional_Tests
        {
            public static IEnumerable<object[]> GetMappers =>
                [
                    [(Func<JsonElement, Result<BlogPost>>)(x => Map<BlogPost>(x, null, null))],
                    [(Func<JsonElement, Result<BlogPost>>)(x => BlogPost.MapFromJson(x, null))]
                ];

            [CreateMapper]
            public partial class BlogPost
            {
                /// <summary>
                /// Field must exist
                /// Value can be null
                /// </summary>
                public Option<string> Title { get; set; }
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void ValidSingleField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "Title": "Farting Unicorns"
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.True(blogPost.Success);
                blogPost.Value.Title.Should().BeOfType<Some<string>>();
                var someTitle = (blogPost.Value.Title as Some<string>)!;
                Assert.Equal("Farting Unicorns", someTitle.Value);
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void MissingNonNullableField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.False(blogPost.Success);
                blogPost.Errors.Should().ContainSingle();
                blogPost.Errors.Single().Should().BeOfType<RequiredPropertyMissingError>();
                blogPost.Errors.Single().Message.Should().Be("$.Title is required");
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void NulledOptionalField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                  "Title": null
                }
                """);
                var blogPost = map(jsonElement);

                Assert.True(blogPost.Success);
                blogPost.Value.Title.Should().BeOfType<None<string>>();
                var someTitle = (blogPost.Value.Title as None<string>)!;
            }
        }

        public partial class NullableOptional_Tests
        {
            public static IEnumerable<object[]> GetMappers =>
            [
                [(Func<JsonElement, Result<BlogPost>>)(x => Map<BlogPost>(x, null, null))],
                [(Func<JsonElement, Result<BlogPost>>)(x => BlogPost.MapFromJson(x, null))]
            ];

            [CreateMapper]
            public partial class BlogPost
            {
                /// <summary>
                /// Field can be missing
                /// Value can be null
                /// </summary>
                public Option<string>? Title { get; set; }
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void ValidSingleField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                  "Title": "Farting Unicorns"
                }
                """);
                var blogPost = map(jsonElement);

                Assert.True(blogPost.Success);
                blogPost.Value.Title.Should().BeOfType<Some<string>>();
                var someTitle = (blogPost.Value.Title as Some<string>)!;
                Assert.Equal("Farting Unicorns", someTitle.Value);
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void MissingOptionalField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                }
                """);
                var blogPost = map(jsonElement);

                Assert.True(blogPost.Success);
                blogPost.Value.Title.Should().BeNull();
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void NulledOptionalField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                  "Title": null
                }
                """);
                var blogPost = map(jsonElement);

                Assert.True(blogPost.Success);
                blogPost.Value.Title.Should().BeOfType<None<string>>();
                var someTitle = (blogPost.Value.Title as None<string>)!;
            }
        }
    }

    public partial class BoolType
    {
        public partial class NonNullableNonOptional_Tests
        {
            public static IEnumerable<object[]> GetMappers =>
            [
                [(Func<JsonElement, Result<BlogPost>>)(x => Map<BlogPost>(x, null, null))],
                [(Func<JsonElement, Result<BlogPost>>)(x => BlogPost.MapFromJson(x, null, null))]
            ];

            [CreateMapper]
            public partial class BlogPost
            {
                /// <summary>
                /// Field must exist
                /// Value cannot be null
                /// </summary>
                public bool IsDraft { get; set; }
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void ValidSingleField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "IsDraft": true
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.True(blogPost.Success);
                blogPost.Value.IsDraft.Should().BeTrue();
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void MissingNonNullableField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.False(blogPost.Success);
                blogPost.Errors.Should().ContainSingle();
                blogPost.Errors.Single().Should().BeOfType<RequiredPropertyMissingError>();
                blogPost.Errors.Single().Message.Should().Be("$.IsDraft is required");
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void NulledNonNullableField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "IsDraft": null
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.False(blogPost.Success);
                blogPost.Errors.Should().ContainSingle();
                blogPost.Errors.Single().Should().BeOfType<RequiredValueMissingError>();
                blogPost.Errors.Single().Message.Should().Be("$.IsDraft must have a value");
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void InvalidFieldType(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "IsDraft": 123456
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.False(blogPost.Success);
                blogPost.Errors.Should().ContainSingle();
                blogPost.Errors.Single().Should().BeOfType<ValueHasWrongTypeError>();
                blogPost.Errors.Single().Message.Should().Be("Value of $.IsDraft has the wrong type. Expected Boolean, got Number");
            }
        }

        public partial class NullableNonOptional_Tests
        {
            public static IEnumerable<object[]> GetMappers =>
            [
                [(Func<JsonElement, Result<BlogPost>>)(x => Map<BlogPost>(x, null, null))],
                [(Func<JsonElement, Result<BlogPost>>)(x => BlogPost.MapFromJson(x, null, null))]
            ];

            [CreateMapper]
            public partial class BlogPost
            {
                /// <summary>
                /// Field can be missing
                /// Value cannot be null
                /// </summary>
                public bool? IsDraft { get; set; }
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void ValidSingleField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "IsDraft": true
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.True(blogPost.Success);
                blogPost.Value.IsDraft.Should().BeTrue();
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void MissingNullableField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.True(blogPost.Success);
                blogPost.Value.IsDraft.Should().BeNull();
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void NulledNullableField(Func<JsonElement, Result<BlogPost>> map)
            {
                // Might seem counterintuitive, but remember, we said that 
                // clr-null should reflect a missing field. In this case, the field exists, but does not have a value.
                // This would be equivalent to None in an Option type.
                // Therefor, this is not valid json for a nullable field. 
                // If the field exists, it must have a value!
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "IsDraft": null
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.False(blogPost.Success);
                blogPost.Errors.Should().ContainSingle();
                blogPost.Errors.Single().Should().BeOfType<RequiredValueMissingError>();
                blogPost.Errors.Single().Message.Should().Be("$.IsDraft must have a value");
            }
        }

        public partial class NonNullableOptional_Tests
        {
            public static IEnumerable<object[]> GetMappers =>
            [
                [(Func<JsonElement, Result<BlogPost>>)(x => Map<BlogPost>(x, null, null))],
                [(Func<JsonElement, Result<BlogPost>>)(x => BlogPost.MapFromJson(x, null))]
            ];
            [CreateMapper]
            public partial class BlogPost
            {
                /// <summary>
                /// Field must exist
                /// Value can be null
                /// </summary>
                public Option<bool> IsDraft { get; set; }
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void ValidSingleField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "IsDraft": true
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.True(blogPost.Success);
                blogPost.Value.IsDraft.Should().BeOfType<Some<bool>>();
                var someIsDraft = (blogPost.Value.IsDraft as Some<bool>)!;
                someIsDraft.Value.Should().BeTrue();
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void MissingNonNullableField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.False(blogPost.Success);
                blogPost.Errors.Should().ContainSingle();
                blogPost.Errors.Single().Should().BeOfType<RequiredPropertyMissingError>();
                blogPost.Errors.Single().Message.Should().Be("$.IsDraft is required");
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void NulledOptionalField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "IsDraft": null
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.True(blogPost.Success);
                blogPost.Value.IsDraft.Should().BeOfType<None<bool>>();
            }
        }

        public partial class NullableOptional_Tests
        {
            public static IEnumerable<object[]> GetMappers =>
            [
                [(Func<JsonElement, Result<BlogPost>>)(x => Map<BlogPost>(x, null, null))],
                [(Func<JsonElement, Result<BlogPost>>)(x => BlogPost.MapFromJson(x, null))]
            ];

            [CreateMapper]
            public partial class BlogPost
            {
                /// <summary>
                /// Field can be missing
                /// Value can be null
                /// </summary>
                public Option<bool>? IsDraft { get; set; }
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void ValidSingleField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "IsDraft": true
                    }
                    """);
                var blogPost = map(jsonElement);

                blogPost.Should().BeSuccessful();
                blogPost.Value.IsDraft.Should().BeOfType<Some<bool>>();
                var someIsDraft = (blogPost.Value.IsDraft as Some<bool>)!;
                someIsDraft.Value.Should().BeTrue();
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void MissingOptionalField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                    }
                    """);
                var blogPost = map(jsonElement);

                blogPost.Should().BeSuccessful();
                blogPost.Value.IsDraft.Should().BeNull();
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void NulledOptionalField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "IsDraft": null
                    }
                    """);
                var blogPost = map(jsonElement);

                blogPost.Should().BeSuccessful();
                blogPost.Value.IsDraft.Should().BeOfType<None<bool>>();
            }
        }
    }

    public partial class IntType
    {
        public partial class NonNullableNonOptional_Tests
        {
            public static IEnumerable<object[]> GetMappers =>
            [
                [(Func<JsonElement, Result<BlogPost>>)(x => Map<BlogPost>(x, null, null))],
                [(Func<JsonElement, Result<BlogPost>>)(x => BlogPost.MapFromJson(x, null))]
            ];

            [CreateMapper]
            public partial class BlogPost
            {
                /// <summary>
                /// Field must exist
                /// Value cannot be null
                /// </summary>
                public int Rating { get; set; }
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void ValidSingleField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "Rating": 5
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.True(blogPost.Success);
                blogPost.Value.Rating.Should().Be(5);
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void MissingNonNullableField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.False(blogPost.Success);
                blogPost.Errors.Should().ContainSingle();
                blogPost.Errors.Single().Should().BeOfType<RequiredPropertyMissingError>();
                blogPost.Errors.Single().Message.Should().Be("$.Rating is required");
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void NulledNonNullableField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "Rating": null
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.False(blogPost.Success);
                blogPost.Errors.Should().ContainSingle();
                blogPost.Errors.Single().Should().BeOfType<RequiredValueMissingError>();
                blogPost.Errors.Single().Message.Should().Be("$.Rating must have a value");
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void InvalidFieldType(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "Rating": true
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.False(blogPost.Success);
                blogPost.Errors.Should().ContainSingle();
                blogPost.Errors.Single().Should().BeOfType<ValueHasWrongTypeError>();
                blogPost.Errors.Single().Message.Should().Be("Value of $.Rating has the wrong type. Expected Number, got True");
            }
        }

        public partial class NullableNonOptional_Tests
        {
            public static IEnumerable<object[]> GetMappers =>
            [
                [(Func<JsonElement, Result<BlogPost>>)(x => Map<BlogPost>(x, null, null))],
                [(Func<JsonElement, Result<BlogPost>>)(x => BlogPost.MapFromJson(x, null))]
            ];

            [CreateMapper]
            public partial class BlogPost
            {
                /// <summary>
                /// Field can be missing
                /// Value cannot be null
                /// </summary>
                public int? Rating { get; set; }
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void ValidSingleField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "Rating": 5
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.True(blogPost.Success);
                blogPost.Value.Rating.Should().Be(5);
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void MissingNullableField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.True(blogPost.Success);
                blogPost.Value.Rating.Should().BeNull();
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void NulledNullableField(Func<JsonElement, Result<BlogPost>> map)
            {
                // Might seem counterintuitive, but remember, we said that 
                // clr-null should reflect a missing field. In this case, the field exists, but does not have a value.
                // This would be equivalent to None in an Option type.
                // Therefor, this is not valid json for a nullable field. 
                // If the field exists, it must have a value!
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "Rating": null
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.False(blogPost.Success);
                blogPost.Errors.Should().ContainSingle();
                blogPost.Errors.Single().Should().BeOfType<RequiredValueMissingError>();
                blogPost.Errors.Single().Message.Should().Be("$.Rating must have a value");
            }
        }

        public partial class NonNullableOptional_Tests
        {
            public static IEnumerable<object[]> GetMappers =>
            [
                [(Func<JsonElement, Result<BlogPost>>)(x => Map<BlogPost>(x, null, null))],
                [(Func<JsonElement, Result<BlogPost>>)(x => BlogPost.MapFromJson(x, null))]
            ];
            [CreateMapper]
            public partial class BlogPost
            {
                /// <summary>
                /// Field must exist
                /// Value can be null
                /// </summary>
                public Option<int> Rating { get; set; }
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void ValidSingleField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "Rating": 5
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.True(blogPost.Success);
                blogPost.Value.Rating.Should().BeOfType<Some<int>>();
                var someIsDraft = (blogPost.Value.Rating as Some<int>)!;
                someIsDraft.Value.Should().Be(5);
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void MissingNonNullableField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.False(blogPost.Success);
                blogPost.Errors.Should().ContainSingle();
                blogPost.Errors.Single().Should().BeOfType<RequiredPropertyMissingError>();
                blogPost.Errors.Single().Message.Should().Be("$.Rating is required");
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void NulledOptionalField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "Rating": null
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.True(blogPost.Success);
                blogPost.Value.Rating.Should().BeOfType<None<int>>();
            }
        }

        public partial class NullableOptional_Tests
        {
            public static IEnumerable<object[]> GetMappers =>
            [
                [(Func<JsonElement, Result<BlogPost>>)(x => Map<BlogPost>(x, null, null))],
                [(Func<JsonElement, Result<BlogPost>>)(x => BlogPost.MapFromJson(x, null))]
            ];

            [CreateMapper]
            public partial class BlogPost
            {
                /// <summary>
                /// Field can be missing
                /// Value can be null
                /// </summary>
                public Option<int>? Rating { get; set; }
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void ValidSingleField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "Rating": 5
                    }
                    """);
                var blogPost = map(jsonElement);

                blogPost.Should().BeSuccessful();
                blogPost.Value.Rating.Should().BeOfType<Some<int>>();
                var someIsDraft = (blogPost.Value.Rating as Some<int>)!;
                someIsDraft.Value.Should().Be(5);
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void MissingOptionalField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                    }
                    """);
                var blogPost = map(jsonElement);

                blogPost.Should().BeSuccessful();
                blogPost.Value.Rating.Should().BeNull();
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void NulledOptionalField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "Rating": null
                    }
                    """);
                var blogPost = map(jsonElement);

                blogPost.Should().BeSuccessful();
                blogPost.Value.Rating.Should().BeOfType<None<int>>();
            }
        }
    }

    public partial class EnumType
    {

        public partial class NonNullableNonOptional_Tests
        {

            public static IEnumerable<object[]> GetMappers
            {
                get
                {
                    var mapperOptions = new MapperOptions();
                    mapperOptions.AddConverter(new EnumAsStringConverter());
                    return [
                        [(Func<JsonElement, Result<BlogPost>>)(x => Map<BlogPost>(x, mapperOptions, null))],
                        [(Func<JsonElement, Result<BlogPost>>)(x => BlogPost.MapFromJson(x, mapperOptions, null))]
                    ];
                }
            }

            [CreateMapper]
            public partial class BlogPost
            {
                public enum BlogPostStatus { Draft, Published }

                /// <summary>
                /// Field must exist
                /// Value cannot be null
                /// </summary>
                public BlogPostStatus Status { get; set; }
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void ValidSingleField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "Status": "Published"
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.True(blogPost.Success);
                blogPost.Value.Status.Should().Be(BlogPost.BlogPostStatus.Published);
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void MissingNonNullableField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.False(blogPost.Success);
                blogPost.Errors.Should().ContainSingle();
                blogPost.Errors.Single().Should().BeOfType<RequiredPropertyMissingError>();
                blogPost.Errors.Single().Message.Should().Be("$.Status is required");
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void NulledNonNullableField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "Status": null
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.False(blogPost.Success);
                blogPost.Errors.Should().ContainSingle();
                blogPost.Errors.Single().Should().BeOfType<RequiredValueMissingError>();
                blogPost.Errors.Single().Message.Should().Be("$.Status must have a value");
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void InvalidFieldType(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "Status": true
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.False(blogPost.Success);
                blogPost.Errors.Should().ContainSingle();
                blogPost.Errors.Single().Should().BeOfType<ValueHasWrongTypeError>();
                blogPost.Errors.Single().Message.Should().Be("Value of $.Status has the wrong type. Expected String, got True");
            }
        }

        public partial class NullableNonOptional_Tests
        {
            public static IEnumerable<object[]> GetMappers
            {
                get
                {
                    var mapperOptions = new MapperOptions();
                    mapperOptions.AddConverter(new EnumAsStringConverter());
                    return [
                        [(Func<JsonElement, Result<BlogPost>>)(x => Map<BlogPost>(x, mapperOptions, null))],
                        [(Func<JsonElement, Result<BlogPost>>)(x => BlogPost.MapFromJson(x, mapperOptions, null))]
                    ];
                }
            }

            [CreateMapper]
            public partial class BlogPost
            {
                public enum BlogPostStatus { Draft, Published }

                /// <summary>
                /// Field can be missing
                /// Value cannot be null
                /// </summary>
                public BlogPostStatus? Status { get; set; }
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void ValidSingleField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "Status": "Published"
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.True(blogPost.Success);
                blogPost.Value.Status.Should().Be(BlogPost.BlogPostStatus.Published);
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void MissingNullableField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.True(blogPost.Success);
                blogPost.Value.Status.Should().BeNull();
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void NulledNullableField(Func<JsonElement, Result<BlogPost>> map)
            {
                // Might seem counterintuitive, but remember, we said that 
                // clr-null should reflect a missing field. In this case, the field exists, but does not have a value.
                // This would be equivalent to None in an Option type.
                // Therefor, this is not valid json for a nullable field. 
                // If the field exists, it must have a value!
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "Status": null
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.False(blogPost.Success);
                blogPost.Errors.Should().ContainSingle();
                blogPost.Errors.Single().Should().BeOfType<RequiredValueMissingError>();
                blogPost.Errors.Single().Message.Should().Be("$.Status must have a value");
            }
        }

        public partial class NonNullableOptional_Tests
        {
            public static IEnumerable<object[]> GetMappers
            {
                get
                {
                    var mapperOptions = new MapperOptions();
                    mapperOptions.AddConverter(new EnumAsStringConverter());
                    return [
                        [(Func<JsonElement, Result<BlogPost>>)(x => Map<BlogPost>(x, mapperOptions, null))],
                        [(Func<JsonElement, Result<BlogPost>>)(x => BlogPost.MapFromJson(x, mapperOptions, null))]
                    ];
                }
            }

            [CreateMapper]
            public partial class BlogPost
            {
                public enum BlogPostStatus { Draft, Published }
                /// <summary>
                /// Field must exist
                /// Value can be null
                /// </summary>
                public Option<BlogPostStatus> Status { get; set; }
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void ValidSingleField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "Status": "Published"
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.True(blogPost.Success);
                blogPost.Value.Status.Should().BeOfType<Some<BlogPost.BlogPostStatus>>();
                var someIsDraft = (blogPost.Value.Status as Some<BlogPost.BlogPostStatus>)!;
                someIsDraft.Value.Should().Be(BlogPost.BlogPostStatus.Published);
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void MissingNonNullableField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.False(blogPost.Success);
                blogPost.Errors.Should().ContainSingle();
                blogPost.Errors.Single().Should().BeOfType<RequiredPropertyMissingError>();
                blogPost.Errors.Single().Message.Should().Be("$.Status is required");
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void NulledOptionalField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "Status": null
                    }
                    """);
                var blogPost = map(jsonElement);

                Assert.True(blogPost.Success);
                blogPost.Value.Status.Should().BeOfType<None<BlogPost.BlogPostStatus>>();
            }
        }

        public partial class NullableOptional_Tests
        {
            public static IEnumerable<object[]> GetMappers
            {
                get
                {
                    var mapperOptions = new MapperOptions();
                    mapperOptions.AddConverter(new EnumAsStringConverter());
                    return [
                        [(Func<JsonElement, Result<BlogPost>>)(x => Map<BlogPost>(x, mapperOptions, null))],
                        [(Func<JsonElement, Result<BlogPost>>)(x => BlogPost.MapFromJson(x, mapperOptions, null))]
                    ];
                }
            }

            [CreateMapper]
            public partial class BlogPost
            {
                public enum BlogPostStatus { Draft, Published }
                /// <summary>
                /// Field can be missing
                /// Value can be null
                /// </summary>
                public Option<BlogPostStatus>? Status { get; set; }
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void ValidSingleField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "Status": "Published"
                    }
                    """);
                var blogPost = map(jsonElement);

                blogPost.Should().BeSuccessful();
                blogPost.Value.Status.Should().BeOfType<Some<BlogPost.BlogPostStatus>>();
                var someIsDraft = (blogPost.Value.Status as Some<BlogPost.BlogPostStatus>)!;
                someIsDraft.Value.Should().Be(BlogPost.BlogPostStatus.Published);
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void MissingOptionalField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                    }
                    """);
                var blogPost = map(jsonElement);

                blogPost.Should().BeSuccessful();
                blogPost.Value.Status.Should().BeNull();
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void NulledOptionalField(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "Status": null
                    }
                    """);
                var blogPost = map(jsonElement);

                blogPost.Should().BeSuccessful();
                blogPost.Value.Status.Should().BeOfType<None<BlogPost.BlogPostStatus>>();
            }
        }
    }
}
