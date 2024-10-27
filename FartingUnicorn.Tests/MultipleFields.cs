using FluentAssertions;

using System.Text.Json;

using Xunit;

namespace FartingUnicorn.Tests;

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
                blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Title is required");
                blogPost.Errors.Should().ContainSingle(e => e.Message == "$.IsDraft is required");
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
                blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Title is required");
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
                blogPost.Errors.Should().ContainSingle(e => e.Message == "$.IsDraft is required");
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
                blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Title must have a value");
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
                blogPost.Errors.Should().ContainSingle(e => e.Message == "$.IsDraft must have a value");
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
                blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Title must have a value");
                blogPost.Errors.Should().ContainSingle(e => e.Message == "$.IsDraft must have a value");
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
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of $.Title has the wrong type. Expected String, got Number");
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
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of $.IsDraft has the wrong type. Expected Boolean, got String");
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
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of $.Title has the wrong type. Expected String, got Number");
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of $.IsDraft has the wrong type. Expected Boolean, got String");
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
                blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Title must have a value");
                blogPost.Errors.Should().ContainSingle(e => e.Message == "$.IsDraft must have a value");
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
                blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Category is required");
                blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Rating is required");
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
                blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Category is required");
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
                blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Rating is required");
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
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of $.Category has the wrong type. Expected String, got Number");
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
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of $.Rating has the wrong type. Expected Number, got String");
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
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of $.Category has the wrong type. Expected String, got Number");
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of $.Rating has the wrong type. Expected Number, got String");
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
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of $.Rating has the wrong type. Expected Number, got String");
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
                blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Category is required");
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of $.Rating has the wrong type. Expected Number, got String");
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
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of $.Rating has the wrong type. Expected Number, got String");
            }

        }
    }
}
