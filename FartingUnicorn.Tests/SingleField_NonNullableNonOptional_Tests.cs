using FluentAssertions;

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
                blogPost.Errors.Single().Message.Should().Be("Title is required");
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
                blogPost.Errors.Single().Message.Should().Be("Title must have a value");
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
                blogPost.Errors.Single().Message.Should().Be("Value of Title has the wrong type. Expected String, got Number");
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
                blogPost.Errors.Single().Message.Should().Be("Title must have a value");
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
                blogPost.Errors.Single().Message.Should().Be("Title is required");
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
                blogPost.Errors.Single().Message.Should().Be("IsDraft is required");
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
                blogPost.Errors.Single().Message.Should().Be("IsDraft must have a value");
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
                blogPost.Errors.Single().Message.Should().Be("Value of IsDraft has the wrong type. Expected Boolean, got Number");
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
                blogPost.Errors.Single().Message.Should().Be("IsDraft must have a value");
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
                blogPost.Errors.Single().Message.Should().Be("IsDraft is required");
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

                blogPost.Success.Should().BeTrue();
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

                blogPost.Success.Should().BeTrue();
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

                blogPost.Success.Should().BeTrue();
                blogPost.Value.IsDraft.Should().BeOfType<None<bool>>();
            }
        }
    }
}

public class MultipleFields
{
    public class WithoutOptions
    {

        public class PUT
        {
            public class BlogPost
            {
                public string Title { get; set; }
                public bool IsDraft { get; set; }
            }

            [Fact]
            public void BothValid()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
        {
          "Title": "Farting Unicorns",
          "IsDraft": true
        }
        """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                blogPost.Success.Should().BeTrue();
                blogPost.Value.Title.Should().Be("Farting Unicorns");
                blogPost.Value.IsDraft.Should().BeTrue();
            }

            [Fact]
            public void BothMissing()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
        {
        }
        """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().HaveCount(2);
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Title is required");
                blogPost.Errors.Should().ContainSingle(e => e.Message == "IsDraft is required");
            }

            [Fact]
            public void TitleMissing()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
        {
          "IsDraft": true
        }
        """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Title is required");
            }

            [Fact]
            public void IsDraftMissing()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
        {
          "Title": "Farting Unicorns"
        }
        """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().ContainSingle(e => e.Message == "IsDraft is required");
            }

            [Fact]
            public void TitleNulled()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
        {
          "Title": null,
          "IsDraft": true
        }
        """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Title must have a value");
            }

            [Fact]
            public void IsDraftNulled()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
        {
          "Title": "Farting Unicorns",
          "IsDraft": null
        }
        """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().ContainSingle(e => e.Message == "IsDraft must have a value");
            }

            [Fact]
            public void BothNulled()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
        {
          "Title": null,
          "IsDraft": null
        }
        """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().HaveCount(2);
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Title must have a value");
                blogPost.Errors.Should().ContainSingle(e => e.Message == "IsDraft must have a value");
            }

            [Fact]
            public void TitleInvalidType()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
        {
          "Title": 123456,
          "IsDraft": true
        }
        """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of Title has the wrong type. Expected String, got Number");
            }

            [Fact]
            public void IsDraftInvalidType()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
        {
          "Title": "Farting Unicorns",
          "IsDraft": "true"
        }
        """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of IsDraft has the wrong type. Expected Boolean, got String");
            }

            [Fact]
            public void BothInvalidType()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
        {
          "Title": 123456,
          "IsDraft": "true"
        }
        """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().HaveCount(2);
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of Title has the wrong type. Expected String, got Number");
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of IsDraft has the wrong type. Expected Boolean, got String");
            }
        }

        public class PATCH
        {
            public class BlogPost
            {
                public string? Title { get; set; }
                public bool? IsDraft { get; set; }
            }

            [Fact]
            public void BothValidWithValue()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Title": "Farting Unicorns",
                    "IsDraft": true
                }
                """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                blogPost.Success.Should().BeTrue();
                blogPost.Value.Title.Should().Be("Farting Unicorns");
                blogPost.Value.IsDraft.Should().BeTrue();
            }

            [Fact]
            public void BothMissing_AndThatsOk()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                }
                """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                blogPost.Success.Should().BeTrue();
                blogPost.Value.Title.Should().BeNull();
                blogPost.Value.IsDraft.Should().BeNull();
            }

            [Fact]
            public void TitleMissing()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "IsDraft": true
                }
                """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                blogPost.Success.Should().BeTrue();
                blogPost.Value.Title.Should().BeNull();
                blogPost.Value.IsDraft.Should().BeTrue();
            }

            [Fact]
            public void BothNulled_ValuesMustExist()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Title": null,
                    "IsDraft": null
                }
                """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().HaveCount(2);
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Title must have a value");
                blogPost.Errors.Should().ContainSingle(e => e.Message == "IsDraft must have a value");
            }

        }
    }

    public class WithOptions
    {
        public class PUT
        {
            public class BlogPost
            {
                public Option<string> Category { get; set; } // not all blog posts are categorized
                public Option<int> Rating { get; set; } // rating is None before it has been rated first time.
            }

            [Fact]
            public void BothValid_WithValues()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Category": "Farting Unicorns",
                    "Rating": 5
                }
                """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                blogPost.Success.Should().BeTrue();
                blogPost.Value.Category.Should().BeOfType<Some<string>>();
                blogPost.Value.Rating.Should().BeOfType<Some<int>>();
                var someCategory = (blogPost.Value.Category as Some<string>)!;
                var someRating = (blogPost.Value.Rating as Some<int>)!;
                someCategory.Value.Should().Be("Farting Unicorns");
                someRating.Value.Should().Be(5);
            }

            [Fact]
            public void BothMissing_NotOK_BothAreRequiredInPUT()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                }
                """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);
                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().HaveCount(2);
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Category is required");
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Rating is required");
            }

            [Fact]
            public void BothNulled_OK_BothAreOptional()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Category": null,
                    "Rating": null
                }
                """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);
                blogPost.Success.Should().BeTrue();
                blogPost.Value.Category.Should().BeOfType<None<string>>();
                blogPost.Value.Rating.Should().BeOfType<None<int>>();
            }

            [Fact]
            public void CategoryMissing_NotOK()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Rating": 5
                }
                """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);
                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Category is required");
            }

            [Fact]
            public void RatingMissing_NotOK()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Category": "Farting Unicorns"
                }
                """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);
                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Rating is required");
            }

            [Fact]
            public void CategoryNulled_OK()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Category": null,
                    "Rating": 5
                }
                """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);
                blogPost.Success.Should().BeTrue();
                blogPost.Value.Category.Should().BeOfType<None<string>>();
                blogPost.Value.Rating.Should().BeOfType<Some<int>>();
                var someRating = (blogPost.Value.Rating as Some<int>)!;
                someRating.Value.Should().Be(5);
            }

            [Fact]
            public void RatingNulled_OK()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Category": "Farting Unicorns",
                    "Rating": null
                }
                """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);
                blogPost.Success.Should().BeTrue();
                blogPost.Value.Category.Should().BeOfType<Some<string>>();
                var someCategory = (blogPost.Value.Category as Some<string>)!;
                someCategory.Value.Should().Be("Farting Unicorns");
                blogPost.Value.Rating.Should().BeOfType<None<int>>();
            }

            [Fact]
            public void CategoryInvalidType()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Category": 123456,
                    "Rating": 5
                }
                """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);
                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of Category has the wrong type. Expected String, got Number");
            }

            [Fact]
            public void RatingInvalidType()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Category": "Farting Unicorns",
                    "Rating": "5"
                }
                """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);
                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of Rating has the wrong type. Expected Number, got String");
            }

            [Fact]
            public void BothInvalidType()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Category": 123456,
                    "Rating": "5"
                }
                """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);
                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().HaveCount(2);
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of Category has the wrong type. Expected String, got Number");
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of Rating has the wrong type. Expected Number, got String");
            }

            [Fact]
            public void CategoryNulledRatingInvalidType()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Category": null,
                    "Rating": "5"
                }
                """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);
                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of Rating has the wrong type. Expected Number, got String");
            }

            [Fact]
            public void CategoryMissingRatingInvalidType()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Rating": "5"
                }
                """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);
                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().HaveCount(2);
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Category is required");
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of Rating has the wrong type. Expected Number, got String");
            }
        }

        public class PATCH
        {
            public class BlogPost
            {
                public Option<string>? Category { get; set; } // not all blog posts are categorized
                public Option<int>? Rating { get; set; } // rating is None before it has been rated first time.
            }

            [Fact]
            public void BothValid_WithValues()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Category": "Farting Unicorns",
                    "Rating": 5
                }
                """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                blogPost.Success.Should().BeTrue();
                blogPost.Value.Category.Should().BeOfType<Some<string>>();
                blogPost.Value.Rating.Should().BeOfType<Some<int>>();
                var someCategory = (blogPost.Value.Category as Some<string>)!;
                var someRating = (blogPost.Value.Rating as Some<int>)!;
                someCategory.Value.Should().Be("Farting Unicorns");
                someRating.Value.Should().Be(5);
            }

            [Fact]
            public void BothMissing_AndThatsOK()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                }
                """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                blogPost.Success.Should().BeTrue();
                blogPost.Value.Category.Should().BeNull();
                blogPost.Value.Rating.Should().BeNull();
            }

            [Fact]
            public void CategoryMissing_OK_ThatMeansDontTouchIt()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Rating": 5
                }
                """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                blogPost.Success.Should().BeTrue();
                blogPost.Value.Category.Should().BeNull();
                blogPost.Value.Rating.Should().BeOfType<Some<int>>();
                var someRating = (blogPost.Value.Rating as Some<int>)!;
                someRating.Value.Should().Be(5);
            }

            [Fact]
            public void RatingMissing_OK_ThatMeansDontTouchIt()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Category": "Farting Unicorns"
                }
                """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                blogPost.Success.Should().BeTrue();
                blogPost.Value.Category.Should().BeOfType<Some<string>>();
                var someCategory = (blogPost.Value.Category as Some<string>)!;
                someCategory.Value.Should().Be("Farting Unicorns");
                blogPost.Value.Rating.Should().BeNull();
            }

            [Fact]
            public void BothNulled_OK_ThatMeansTheyShouldBeNulled()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Category": null,
                    "Rating": null
                }
                """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);

                blogPost.Success.Should().BeTrue();
                blogPost.Value.Category.Should().BeOfType<None<string>>();
                blogPost.Value.Rating.Should().BeOfType<None<int>>();
            }

            [Fact]
            public void CategoryNulledRatingInvalidType()
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Category": null,
                    "Rating": "5"
                }
                """);
                var blogPost = Mapper.Map<BlogPost>(jsonElement);
                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of Rating has the wrong type. Expected Number, got String");
            }

        }
    }
}

public class Objects
{
    public class NotOptional
    {
        public class BlogPost
        {
            public string Title { get; set; }
            public bool IsDraft { get; set; }
            public Option<string> Category { get; set; }
            public Option<int> Rating { get; set; }
            public Author Author { get; set; }
        }
        public class Author
        {
            public string Name { get; set; }
            public Option<int> Age { get; set; }
        }

        [Fact]
        public void Valid()
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {
                "Title": "Farting Unicorns",
                "IsDraft": true,
                "Category": "Horses",
                "Rating": 5,
                "Author": {
                    "Name": "John Doe",
                    "Age": 42
                }
            }
            """);
            var blogPost = Mapper.Map<BlogPost>(json);
            blogPost.Success.Should().BeTrue();
            blogPost.Value.Title.Should().Be("Farting Unicorns");
            blogPost.Value.IsDraft.Should().BeTrue();
            blogPost.Value.Category.Should().BeOfType<Some<string>>();
            var someCategory = (blogPost.Value.Category as Some<string>)!;
            someCategory.Value.Should().Be("Horses");
            blogPost.Value.Rating.Should().BeOfType<Some<int>>();
            var someRating = (blogPost.Value.Rating as Some<int>)!;
            someRating.Value.Should().Be(5);
            blogPost.Value.Author.Name.Should().Be("John Doe");
            blogPost.Value.Author.Age.Should().BeOfType<Some<int>>();
            var someAge = (blogPost.Value.Author.Age as Some<int>)!;
            someAge.Value.Should().Be(42);
        }

        [Fact]
        public void AgeIsOptional_ButIsNotAllowedToBeMissing()
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {
                "Title": "Farting Unicorns",
                "IsDraft": true,
                "Category": "Horses",
                "Rating": 5,
                "Author": {
                    "Name": "John Doe"
                }
            }
            """);
            var blogPost = Mapper.Map<BlogPost>(json);
            blogPost.Success.Should().BeFalse();
            blogPost.Errors.Should().ContainSingle(e => e.Message == "Author.Age is required");
        }

        [Fact]
        public void AgeIsOptional()
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {
                "Title": "Farting Unicorns",
                "IsDraft": true,
                "Category": "Horses",
                "Rating": 5,
                "Author": {
                    "Name": "John Doe",
                    "Age": null
                }
            }
            """);
            var blogPost = Mapper.Map<BlogPost>(json);
            blogPost.Success.Should().BeTrue();
            blogPost.Value.Author.Age.Should().BeOfType<None<int>>();
        }

        [Fact]
        public void AuthorIsRequired()
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {
                "Title": "Farting Unicorns",
                "IsDraft": true,
                "Category": "Horses",
                "Rating": 5
            }
            """);
            var blogPost = Mapper.Map<BlogPost>(json);
            blogPost.Success.Should().BeFalse();
            blogPost.Errors.Should().ContainSingle(e => e.Message == "Author is required");
        }

        [Fact]
        public void AuthorCannotBeNull()
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {
                "Title": "Farting Unicorns",
                "IsDraft": true,
                "Category": "Horses",
                "Rating": 5,
                "Author": null
            }
            """);
            var blogPost = Mapper.Map<BlogPost>(json);
            blogPost.Success.Should().BeFalse();
            blogPost.Errors.Should().ContainSingle(e => e.Message == "Author must have a value");
        }

        [Fact]
        public void AuthorIsWrongType()
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {
                "Title": "Farting Unicorns",
                "IsDraft": true,
                "Category": "Horses",
                "Rating": 5,
                "Author": 123456
            }
            """);
            var blogPost = Mapper.Map<BlogPost>(json);
            blogPost.Success.Should().BeFalse();
            blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of Author has the wrong type. Expected Object, got Number");

        }
    }

    public class Optional
    {
        public class BlogPost
        {
            public string Title { get; set; }
            public bool IsDraft { get; set; }
            public Option<string> Category { get; set; }
            public Option<int> Rating { get; set; }
            public Option<Author> Author { get; set; }
        }
        public class Author
        {
            public string Name { get; set; }
            public Option<int> Age { get; set; }
        }
        [Fact]
        public void Valid()
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {
                "Title": "Farting Unicorns",
                "IsDraft": true,
                "Category": "Horses",
                "Rating": 5,
                "Author": {
                    "Name": "John Doe",
                    "Age": 42
                }
            }
            """);
            var blogPost = Mapper.Map<BlogPost>(json);
            blogPost.Success.Should().BeTrue();
            blogPost.Value.Title.Should().Be("Farting Unicorns");
            blogPost.Value.IsDraft.Should().BeTrue();
            blogPost.Value.Category.Should().BeOfType<Some<string>>();
            var someCategory = (blogPost.Value.Category as Some<string>)!;
            someCategory.Value.Should().Be("Horses");
            blogPost.Value.Rating.Should().BeOfType<Some<int>>();
            var someRating = (blogPost.Value.Rating as Some<int>)!;
            someRating.Value.Should().Be(5);
            blogPost.Value.Author.Should().BeOfType<Some<Author>>();
            var someAuthor = (blogPost.Value.Author as Some<Author>)!;
            someAuthor.Value.Name.Should().Be("John Doe");
            someAuthor.Value.Age.Should().BeOfType<Some<int>>();
            var someAge = (someAuthor.Value.Age as Some<int>)!;
            someAge.Value.Should().Be(42);
        }

        [Fact]
        public void AgeIsOptional_ButIsNotAllowedToBeMissing()
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {
                "Title": "Farting Unicorns",
                "IsDraft": true,
                "Category": "Horses",
                "Rating": 5,
                "Author": {
                    "Name": "John Doe"
                }
            }
            """);
            var blogPost = Mapper.Map<BlogPost>(json);
            blogPost.Success.Should().BeFalse();
            blogPost.Errors.Should().ContainSingle(e => e.Message == "Author.Age is required");
        }

        [Fact]
        public void AuthorCannoBeMissing()
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {
                "Title": "Farting Unicorns",
                "IsDraft": true,
                "Category": "Horses",
                "Rating": 5
            }
            """);
            var blogPost = Mapper.Map<BlogPost>(json);
            blogPost.Success.Should().BeFalse();
            blogPost.Errors.Should().ContainSingle(e => e.Message == "Author is required");
        }

        [Fact]
        public void AuthorIsOptional()
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {
                "Title": "Farting Unicorns",
                "IsDraft": true,
                "Category": "Horses",
                "Rating": 5,
                "Author": null
            }
            """);
            var blogPost = Mapper.Map<BlogPost>(json);
            blogPost.Success.Should().BeTrue();
            blogPost.Value.Author.Should().BeOfType<None<Author>>();
        }

        [Fact]
        public void AuthorIsWrongType()
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {
                "Title": "Farting Unicorns",
                "IsDraft": true,
                "Category": "Horses",
                "Rating": 5,
                "Author": 123456
            }
            """);
            var blogPost = Mapper.Map<BlogPost>(json);
            blogPost.Success.Should().BeFalse();
            blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of Author has the wrong type. Expected Object, got Number");

        }
    }

    public class Nullable
    {
        public class BlogPost
        {
            public Author? Author { get; set; }
        }
        public class Author
        {
            public string Name { get; set; }
            public Option<int> Age { get; set; }
        }

        [Fact]
        public void Valid()
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {
                "Author": {
                    "Name": "John Doe",
                    "Age": 42
                }
            }
            """);
            var blogPost = Mapper.Map<BlogPost>(json);
            blogPost.Success.Should().BeTrue();
            blogPost.Value.Author.Should().NotBeNull();
            blogPost.Value.Author!.Name.Should().Be("John Doe");
            blogPost.Value.Author!.Age.Should().BeOfType<Some<int>>();
        }

        [Fact]
        public void Missing_OK()
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {
            }
            """);
            var blogPost = Mapper.Map<BlogPost>(json);
            blogPost.Success.Should().BeTrue();
            blogPost.Value.Author.Should().BeNull();
        }
    }

    public class NullableOptional
    {
        public class BlogPost
        {
            public Option<Author>? Author { get; set; }
        }
        public class Author
        {
            public string Name { get; set; }
            public Option<int> Age { get; set; }
        }

        [Fact]
        public void Valid()
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {
                "Author": {
                    "Name": "John Doe",
                    "Age": 42
                }
            }
            """);
            var blogPost = Mapper.Map<BlogPost>(json);
            blogPost.Success.Should().BeTrue();
            blogPost.Value.Author.Should().NotBeNull();
            blogPost.Value.Author!.Should().BeOfType<Some<Author>>();
            var someAuthor = (blogPost.Value.Author as Some<Author>)!;
            someAuthor.Value.Name.Should().Be("John Doe");
            someAuthor.Value.Age.Should().BeOfType<Some<int>>();
        }

        [Fact]
        public void Missing()
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {
            }
            """);
            var blogPost = Mapper.Map<BlogPost>(json);
            blogPost.Success.Should().BeTrue();
            blogPost.Value.Author.Should().BeNull();
        }

        [Fact]
        public void Nulled()
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {
                "Author": null
            }
            """);
            var blogPost = Mapper.Map<BlogPost>(json);
            blogPost.Success.Should().BeTrue();
            blogPost.Value.Author.Should().BeOfType<None<Author>>();
        }

    }

    public class Arrays
    {
        public class NotOptional
        {
            public class BlogPost
            {
                public Comment[] Comments { get; set; }
            }

            public class Comment
            {
                public string Text { get; set; }
                public int Upvotes { get; set; }
                public Option<string> Contact { get; set; }
            }

            [Fact]
            public void Valid()
            {
                var json = JsonSerializer.Deserialize<JsonElement>("""
            {
                "Comments": [
                    {
                        "Text": "First!",
                        "Upvotes": 5,
                        "Contact": "John Doe"
                    },
                    {
                        "Text": "Second!",
                        "Upvotes": 3,
                        "Contact": null
                    }
                ]
            }
            """);
                var blogPost = Mapper.Map<BlogPost>(json);
                blogPost.Success.Should().BeTrue();
                blogPost.Value.Comments.Should().HaveCount(2);
                blogPost.Value.Comments[0].Text.Should().Be("First!");
                blogPost.Value.Comments[0].Upvotes.Should().Be(5);
                blogPost.Value.Comments[0].Contact.Should().BeOfType<Some<string>>();
                var someAuthor = (blogPost.Value.Comments[0].Contact as Some<string>)!;
                someAuthor.Value.Should().Be("John Doe");
                blogPost.Value.Comments[1].Text.Should().Be("Second!");
                blogPost.Value.Comments[1].Upvotes.Should().Be(3);
                blogPost.Value.Comments[1].Contact.Should().BeOfType<None<string>>();
            }

            [Fact]
            public void MissingFields()
            {
                var json = JsonSerializer.Deserialize<JsonElement>("""
            {
                "Comments": [
                    {
                        "Upvotes": 5,
                        "Contact": "John Doe"
                    },
                    {
                        "Text": "Second!",
                        "Upvotes": 3
                    }
                ]
            }
            """);
                var blogPost = Mapper.Map<BlogPost>(json);
                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Comments.0.Text is required");
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Comments.1.Contact is required");

            }

            [Fact]
            public void MissingArray()
            {
                var json = JsonSerializer.Deserialize<JsonElement>("""
            {
            }
            """);
                var blogPost = Mapper.Map<BlogPost>(json);
                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Comments is required");
            }
        }


    }
}