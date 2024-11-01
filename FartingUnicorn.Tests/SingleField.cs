﻿using FluentAssertions;

using System.Text.Json;

using Xunit;

using static FartingUnicorn.Mapper;

namespace FartingUnicorn.Tests;

public class SingleField
{
    public class ReferenceType
    {
        public class NonNullableNonOptional_Tests

        {
            public class BlogPost
            {
                /// <summary>
                /// Field must exist
                /// Value cannot be null
                /// </summary>
                public string Title { get; set; }
            }

            [Fact]
            public void ValidSingleField()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
            {
              "Title": "Farting Unicorns"
            }
            """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                Assert.True(blogPost.Success);
                Assert.Equal("Farting Unicorns", blogPost.Value.Title);
            }

            [Fact]
            public void MissingNonNullableField()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
            {
            }
            """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                Assert.False(blogPost.Success);
                blogPost.Errors.Should().ContainSingle();
                blogPost.Errors.Single().Should().BeOfType<RequiredPropertyMissingError>();
                blogPost.Errors.Single().Message.Should().Be("$.Title is required");
            }

            [Fact]
            public void NulledNonNullableField()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
            {
              "Title": null
            }
            """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                Assert.False(blogPost.Success);
                blogPost.Errors.Should().ContainSingle();
                blogPost.Errors.Single().Should().BeOfType<RequiredValueMissingError>();
                blogPost.Errors.Single().Message.Should().Be("$.Title must have a value");
            }

            [Fact]
            public void InvalidFieldType()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
            {
              "Title": 123456
            }
            """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                Assert.False(blogPost.Success);
                blogPost.Errors.Should().ContainSingle();
                blogPost.Errors.Single().Should().BeOfType<ValueHasWrongTypeError>();
                blogPost.Errors.Single().Message.Should().Be("Value of $.Title has the wrong type. Expected String, got Number");
            }
        }

        public class NullableNonOptional_Tests
        {
            public class BlogPost
            {
                /// <summary>
                /// Field can be missing
                /// Value cannot be null
                /// </summary>
                public string? Title { get; set; }
            }

            [Fact]
            public void ValidSingleField()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
            {
              "Title": "Farting Unicorns"
            }
            """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                Assert.True(blogPost.Success);
                Assert.Equal("Farting Unicorns", blogPost.Value.Title);
            }

            [Fact]
            public void MissingNullableField()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
            {
            }
            """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                Assert.True(blogPost.Success);
                Assert.Null(blogPost.Value.Title);
            }

            [Fact]
            public void NulledNullableField()
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
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                Assert.False(blogPost.Success);
                blogPost.Errors.Should().ContainSingle();
                blogPost.Errors.Single().Should().BeOfType<RequiredValueMissingError>();
                blogPost.Errors.Single().Message.Should().Be("$.Title must have a value");
            }
        }

        public class NonNullableOptional_Tests
        {
            public class BlogPost
            {
                /// <summary>
                /// Field must exist
                /// Value can be null
                /// </summary>
                public Option<string> Title { get; set; }
            }

            [Fact]
            public void ValidSingleField()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
            {
              "Title": "Farting Unicorns"
            }
            """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                Assert.True(blogPost.Success);
                blogPost.Value.Title.Should().BeOfType<Some<string>>();
                var someTitle = (blogPost.Value.Title as Some<string>)!;
                Assert.Equal("Farting Unicorns", someTitle.Value);
            }

            [Fact]
            public void MissingNonNullableField()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
            {
            }
            """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                Assert.False(blogPost.Success);
                blogPost.Errors.Should().ContainSingle();
                blogPost.Errors.Single().Should().BeOfType<RequiredPropertyMissingError>();
                blogPost.Errors.Single().Message.Should().Be("$.Title is required");
            }

            [Fact]
            public void NulledOptionalField()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
            {
              "Title": null
            }
            """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                Assert.True(blogPost.Success);
                blogPost.Value.Title.Should().BeOfType<None<string>>();
                var someTitle = (blogPost.Value.Title as None<string>)!;
            }
        }

        public class NullableOptional_Tests
        {
            public class BlogPost
            {
                /// <summary>
                /// Field can be missing
                /// Value can be null
                /// </summary>
                public Option<string>? Title { get; set; }
            }

            [Fact]
            public void ValidSingleField()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
            {
              "Title": "Farting Unicorns"
            }
            """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                Assert.True(blogPost.Success);
                blogPost.Value.Title.Should().BeOfType<Some<string>>();
                var someTitle = (blogPost.Value.Title as Some<string>)!;
                Assert.Equal("Farting Unicorns", someTitle.Value);
            }

            [Fact]
            public void MissingOptionalField()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
            {
            }
            """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                Assert.True(blogPost.Success);
                blogPost.Value.Title.Should().BeNull();
            }

            [Fact]
            public void NulledOptionalField()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
            {
              "Title": null
            }
            """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                Assert.True(blogPost.Success);
                blogPost.Value.Title.Should().BeOfType<None<string>>();
                var someTitle = (blogPost.Value.Title as None<string>)!;
            }
        }
    }

    public class ValueType
    {
        public class NonNullableNonOptional_Tests

        {
            public class BlogPost
            {
                /// <summary>
                /// Field must exist
                /// Value cannot be null
                /// </summary>
                public bool IsDraft { get; set; }
            }

            [Fact]
            public void ValidSingleField()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
            {
              "IsDraft": true
            }
            """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                Assert.True(blogPost.Success);
                blogPost.Value.IsDraft.Should().BeTrue();
            }

            [Fact]
            public void MissingNonNullableField()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
            {
            }
            """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                Assert.False(blogPost.Success);
                blogPost.Errors.Should().ContainSingle();
                blogPost.Errors.Single().Should().BeOfType<RequiredPropertyMissingError>();
                blogPost.Errors.Single().Message.Should().Be("$.IsDraft is required");
            }

            [Fact]
            public void NulledNonNullableField()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
            {
              "IsDraft": null
            }
            """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                Assert.False(blogPost.Success);
                blogPost.Errors.Should().ContainSingle();
                blogPost.Errors.Single().Should().BeOfType<RequiredValueMissingError>();
                blogPost.Errors.Single().Message.Should().Be("$.IsDraft must have a value");
            }

            [Fact]
            public void InvalidFieldType()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
            {
              "IsDraft": 123456
            }
            """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                Assert.False(blogPost.Success);
                blogPost.Errors.Should().ContainSingle();
                blogPost.Errors.Single().Should().BeOfType<ValueHasWrongTypeError>();
                blogPost.Errors.Single().Message.Should().Be("Value of $.IsDraft has the wrong type. Expected Boolean, got Number");
            }
        }

        public class NullableNonOptional_Tests
        {
            public class BlogPost
            {
                /// <summary>
                /// Field can be missing
                /// Value cannot be null
                /// </summary>
                public bool? IsDraft { get; set; }
            }

            [Fact]
            public void ValidSingleField()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "IsDraft": true
                    }
                    """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                Assert.True(blogPost.Success);
                blogPost.Value.IsDraft.Should().BeTrue();
            }

            [Fact]
            public void MissingNullableField()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
            {
            }
            """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                Assert.True(blogPost.Success);
                blogPost.Value.IsDraft.Should().BeNull();
            }

            [Fact]
            public void NulledNullableField()
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
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                Assert.False(blogPost.Success);
                blogPost.Errors.Should().ContainSingle();
                blogPost.Errors.Single().Should().BeOfType<RequiredValueMissingError>();
                blogPost.Errors.Single().Message.Should().Be("$.IsDraft must have a value");
            }
        }

        public class NonNullableOptional_Tests
        {
            public class BlogPost
            {
                /// <summary>
                /// Field must exist
                /// Value can be null
                /// </summary>
                public Option<bool> IsDraft { get; set; }
            }

            [Fact]
            public void ValidSingleField()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
            {
              "IsDraft": true
            }
            """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                Assert.True(blogPost.Success);
                blogPost.Value.IsDraft.Should().BeOfType<Some<bool>>();
                var someIsDraft = (blogPost.Value.IsDraft as Some<bool>)!;
                someIsDraft.Value.Should().BeTrue();
            }

            [Fact]
            public void MissingNonNullableField()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
            {
            }
            """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                Assert.False(blogPost.Success);
                blogPost.Errors.Should().ContainSingle();
                blogPost.Errors.Single().Should().BeOfType<RequiredPropertyMissingError>();
                blogPost.Errors.Single().Message.Should().Be("$.IsDraft is required");
            }

            [Fact]
            public void NulledOptionalField()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "IsDraft": null
                    }
                    """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                Assert.True(blogPost.Success);
                blogPost.Value.IsDraft.Should().BeOfType<None<bool>>();
            }
        }

        public class NullableOptional_Tests
        {
            public class BlogPost
            {
                /// <summary>
                /// Field can be missing
                /// Value can be null
                /// </summary>
                public Option<bool>? IsDraft { get; set; }
            }

            [Fact]
            public void ValidSingleField()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                    {
                      "IsDraft": true
                    }
                    """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                blogPost.Should().BeSuccessful();
                blogPost.Value.IsDraft.Should().BeOfType<Some<bool>>();
                var someIsDraft = (blogPost.Value.IsDraft as Some<bool>)!;
                someIsDraft.Value.Should().BeTrue();
            }

            [Fact]
            public void MissingOptionalField()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
            {
            }
            """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                blogPost.Should().BeSuccessful();
                blogPost.Value.IsDraft.Should().BeNull();
            }

            [Fact]
            public void NulledOptionalField()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                  "IsDraft": null
                }
                """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                blogPost.Should().BeSuccessful();
                blogPost.Value.IsDraft.Should().BeOfType<None<bool>>();
            }
        }
    }
}
