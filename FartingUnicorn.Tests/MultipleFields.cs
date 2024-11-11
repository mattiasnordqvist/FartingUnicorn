using DotNetThoughts.FartingUnicorn;
using DotNetThoughts.Results;

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
            [CreateMapper]
            public class BlogPost
            {
                public string Title { get; set; }
                public bool IsDraft { get; set; }
            }

            public static IEnumerable<object[]> GetMappers =>
            [
                [(Func<JsonElement, Result<BlogPost>>)(x => Mapper.Map<BlogPost>(x))],
                [(Func<JsonElement, Result<BlogPost>>)(x => Generated.Mappers.MapToFartingUnicorn_Tests_MultipleFields_WithoutOptions_PUT_BlogPost(x))]
            ];

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void BothValid(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                  "Title": "Farting Unicorns",
                  "IsDraft": true
                }
                """);
                var blogPost = map(jsonElement);

                blogPost.Should().BeSuccessful();
                blogPost.Value.Title.Should().Be("Farting Unicorns");
                blogPost.Value.IsDraft.Should().BeTrue();
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void BothMissing(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                }
                """);
                var blogPost = map(jsonElement);

                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().HaveCount(2);
                blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Title is required");
                blogPost.Errors.Should().ContainSingle(e => e.Message == "$.IsDraft is required");
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void TitleMissing(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                  "IsDraft": true
                }
                """);
                var blogPost = map(jsonElement);

                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Title is required");
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void IsDraftMissing(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                  "Title": "Farting Unicorns"
                }
                """);
                var blogPost = map(jsonElement);

                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().ContainSingle(e => e.Message == "$.IsDraft is required");
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void TitleNulled(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                  "Title": null,
                  "IsDraft": true
                }
                """);
                var blogPost = map(jsonElement);

                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Title must have a value");
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void IsDraftNulled(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                  "Title": "Farting Unicorns",
                  "IsDraft": null
                }
                """);
                var blogPost = map(jsonElement);

                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().ContainSingle(e => e.Message == "$.IsDraft must have a value");
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void BothNulled(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                  "Title": null,
                  "IsDraft": null
                }
                """);
                var blogPost = map(jsonElement);

                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().HaveCount(2);
                blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Title must have a value");
                blogPost.Errors.Should().ContainSingle(e => e.Message == "$.IsDraft must have a value");
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void TitleInvalidType(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                  "Title": 123456,
                  "IsDraft": true
                }
                """);
                var blogPost = map(jsonElement);

                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of $.Title has the wrong type. Expected String, got Number");
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void IsDraftInvalidType(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                  "Title": "Farting Unicorns",
                  "IsDraft": "true"
                }
                """);
                var blogPost = map(jsonElement);

                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of $.IsDraft has the wrong type. Expected Boolean, got String");
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void BothInvalidType(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                  "Title": 123456,
                  "IsDraft": "true"
                }
                """);
                var blogPost = map(jsonElement);

                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().HaveCount(2);
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of $.Title has the wrong type. Expected String, got Number");
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of $.IsDraft has the wrong type. Expected Boolean, got String");
            }
        }

        public class PATCH
        {
            [CreateMapper]
            public class BlogPost
            {
                public string? Title { get; set; }
                public bool? IsDraft { get; set; }
            }

            public static IEnumerable<object[]> GetMappers =>
            [
                [(Func<JsonElement, Result<BlogPost>>)(x => Mapper.Map<BlogPost>(x))],
                [(Func<JsonElement, Result<BlogPost>>)(x => Generated.Mappers.MapToFartingUnicorn_Tests_MultipleFields_WithoutOptions_PATCH_BlogPost(x))]
            ];

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void BothValidWithValue(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Title": "Farting Unicorns",
                    "IsDraft": true
                }
                """);
                var blogPost = map(jsonElement);

                blogPost.Should().BeSuccessful();
                blogPost.Value.Title.Should().Be("Farting Unicorns");
                blogPost.Value.IsDraft.Should().BeTrue();
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void BothMissing_AndThatsOk(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                }
                """);
                var blogPost = map(jsonElement);

                blogPost.Should().BeSuccessful();
                blogPost.Value.Title.Should().BeNull();
                blogPost.Value.IsDraft.Should().BeNull();
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void TitleMissing(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "IsDraft": true
                }
                """);
                var blogPost = map(jsonElement);

                blogPost.Should().BeSuccessful();
                blogPost.Value.Title.Should().BeNull();
                blogPost.Value.IsDraft.Should().BeTrue();
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void BothNulled_ValuesMustExist(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Title": null,
                    "IsDraft": null
                }
                """);
                var blogPost = map(jsonElement);

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
            [CreateMapper]
            public class BlogPost
            {
                public Option<string> Category { get; set; } // not all blog posts are categorized
                public Option<int> Rating { get; set; } // rating is None before it has been rated first time.
            }

            public static IEnumerable<object[]> GetMappers =>
            [
                [(Func<JsonElement, Result<BlogPost>>)(x => Mapper.Map<BlogPost>(x))],
                [(Func<JsonElement, Result<BlogPost>>)(x => Generated.Mappers.MapToFartingUnicorn_Tests_MultipleFields_WithOptions_PUT_BlogPost(x))]
            ];


            [Theory]
            [MemberData(nameof(GetMappers))]
            public void BothValid_WithValues(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Category": "Farting Unicorns",
                    "Rating": 5
                }
                """);
                var blogPost = map(jsonElement);

                blogPost.Should().BeSuccessful();
                blogPost.Value.Category.Should().BeOfType<Some<string>>();
                blogPost.Value.Rating.Should().BeOfType<Some<int>>();
                var someCategory = (blogPost.Value.Category as Some<string>)!;
                var someRating = (blogPost.Value.Rating as Some<int>)!;
                someCategory.Value.Should().Be("Farting Unicorns");
                someRating.Value.Should().Be(5);
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void BothMissing_NotOK_BothAreRequiredInPUT(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                }
                """);
                var blogPost = map(jsonElement);
                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().HaveCount(2);
                blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Category is required");
                blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Rating is required");
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void BothNulled_OK_BothAreOptional(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Category": null,
                    "Rating": null
                }
                """);
                var blogPost = map(jsonElement);
                blogPost.Should().BeSuccessful();
                blogPost.Value.Category.Should().BeOfType<None<string>>();
                blogPost.Value.Rating.Should().BeOfType<None<int>>();
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void CategoryMissing_NotOK(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Rating": 5
                }
                """);
                var blogPost = map(jsonElement);
                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Category is required");
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void RatingMissing_NotOK(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Category": "Farting Unicorns"
                }
                """);
                var blogPost = map(jsonElement);
                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Rating is required");
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void CategoryNulled_OK(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Category": null,
                    "Rating": 5
                }
                """);
                var blogPost = map(jsonElement);
                blogPost.Should().BeSuccessful();
                blogPost.Value.Category.Should().BeOfType<None<string>>();
                blogPost.Value.Rating.Should().BeOfType<Some<int>>();
                var someRating = (blogPost.Value.Rating as Some<int>)!;
                someRating.Value.Should().Be(5);
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void RatingNulled_OK(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Category": "Farting Unicorns",
                    "Rating": null
                }
                """);
                var blogPost = map(jsonElement);
                blogPost.Should().BeSuccessful();
                blogPost.Value.Category.Should().BeOfType<Some<string>>();
                var someCategory = (blogPost.Value.Category as Some<string>)!;
                someCategory.Value.Should().Be("Farting Unicorns");
                blogPost.Value.Rating.Should().BeOfType<None<int>>();
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void CategoryInvalidType(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Category": 123456,
                    "Rating": 5
                }
                """);
                var blogPost = map(jsonElement);
                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of $.Category has the wrong type. Expected String, got Number");
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void RatingInvalidType(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Category": "Farting Unicorns",
                    "Rating": "5"
                }
                """);
                var blogPost = map(jsonElement);
                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of $.Rating has the wrong type. Expected Number, got String");
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void BothInvalidType(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Category": 123456,
                    "Rating": "5"
                }
                """);
                var blogPost = map(jsonElement);
                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().HaveCount(2);
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of $.Category has the wrong type. Expected String, got Number");
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of $.Rating has the wrong type. Expected Number, got String");
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void CategoryNulledRatingInvalidType(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Category": null,
                    "Rating": "5"
                }
                """);
                var blogPost = map(jsonElement);
                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of $.Rating has the wrong type. Expected Number, got String");
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void CategoryMissingRatingInvalidType(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Rating": "5"
                }
                """);
                var blogPost = map(jsonElement);
                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().HaveCount(2);
                blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Category is required");
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of $.Rating has the wrong type. Expected Number, got String");
            }
        }

        public class PATCH
        {
            [CreateMapper]
            public class BlogPost
            {
                public Option<string>? Category { get; set; } // not all blog posts are categorized
                public Option<int>? Rating { get; set; } // rating is None before it has been rated first time.
            }

            public static IEnumerable<object[]> GetMappers =>
            [
                [(Func<JsonElement, Result<BlogPost>>)(x => Mapper.Map<BlogPost>(x))],
                [(Func<JsonElement, Result<BlogPost>>)(x => Generated.Mappers.MapToFartingUnicorn_Tests_MultipleFields_WithOptions_PATCH_BlogPost(x))]
            ];

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void BothValid_WithValues(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Category": "Farting Unicorns",
                    "Rating": 5
                }
                """);
                var blogPost = map(jsonElement);

                blogPost.Should().BeSuccessful();
                blogPost.Value.Category.Should().BeOfType<Some<string>>();
                blogPost.Value.Rating.Should().BeOfType<Some<int>>();
                var someCategory = (blogPost.Value.Category as Some<string>)!;
                var someRating = (blogPost.Value.Rating as Some<int>)!;
                someCategory.Value.Should().Be("Farting Unicorns");
                someRating.Value.Should().Be(5);
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void BothMissing_AndThatsOK(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                }
                """);
                var blogPost = map(jsonElement);

                blogPost.Should().BeSuccessful();
                blogPost.Value.Category.Should().BeNull();
                blogPost.Value.Rating.Should().BeNull();
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void CategoryMissing_OK_ThatMeansDontTouchIt(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Rating": 5
                }
                """);
                var blogPost = map(jsonElement);

                blogPost.Should().BeSuccessful();
                blogPost.Value.Category.Should().BeNull();
                blogPost.Value.Rating.Should().BeOfType<Some<int>>();
                var someRating = (blogPost.Value.Rating as Some<int>)!;
                someRating.Value.Should().Be(5);
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void RatingMissing_OK_ThatMeansDontTouchIt(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Category": "Farting Unicorns"
                }
                """);
                var blogPost = map(jsonElement);

                blogPost.Should().BeSuccessful();
                blogPost.Value.Category.Should().BeOfType<Some<string>>();
                var someCategory = (blogPost.Value.Category as Some<string>)!;
                someCategory.Value.Should().Be("Farting Unicorns");
                blogPost.Value.Rating.Should().BeNull();
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void BothNulled_OK_ThatMeansTheyShouldBeNulled(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Category": null,
                    "Rating": null
                }
                """);
                var blogPost = map(jsonElement);

                blogPost.Should().BeSuccessful();
                blogPost.Value.Category.Should().BeOfType<None<string>>();
                blogPost.Value.Rating.Should().BeOfType<None<int>>();
            }

            [Theory]
            [MemberData(nameof(GetMappers))]
            public void CategoryNulledRatingInvalidType(Func<JsonElement, Result<BlogPost>> map)
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>("""
                {
                    "Category": null,
                    "Rating": "5"
                }
                """);
                var blogPost = map(jsonElement);
                blogPost.Success.Should().BeFalse();
                blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of $.Rating has the wrong type. Expected Number, got String");
            }

        }
    }
}
