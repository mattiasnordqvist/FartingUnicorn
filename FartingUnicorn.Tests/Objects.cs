using DotNetThoughts.FartingUnicorn;
using DotNetThoughts.Results;

using FluentAssertions;

using System.Text.Json;

using Xunit;

namespace FartingUnicorn.Tests;

// Just here to make sure the generated code compiles
public enum Gender { Male, Female, Other }

[CreateMapper]
public partial record MyRecord(string Name, int Age, Option<Gender> Gender, string? Pet);

public partial class Objects
{
    public partial class NotOptional
    {

        [CreateMapper]
        public partial class BlogPost
        {
            public string Title { get; set; }
            public bool IsDraft { get; set; }
            public Option<string> Category { get; set; }
            public Option<int> Rating { get; set; }
            public Author Author { get; set; }
        }

        public partial class Author
        {
            public string Name { get; set; }
            public Option<int> Age { get; set; }
        }

        public static IEnumerable<object[]> GetMappers =>
        [
            [(Func<JsonElement, Result<BlogPost>>)(x => Mapper.Map<BlogPost>(x))],
            [(Func<JsonElement, Result<BlogPost>>)(x => BlogPost.MapFromJson(x))]
        ];

        [Theory, MemberData(nameof(GetMappers))]
        public void Valid(Func<JsonElement, Result<BlogPost>> map)
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
            var blogPost = map(json);
            blogPost.Should().BeSuccessful();
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

        [Theory, MemberData(nameof(GetMappers))]
        public void AgeIsOptional_ButIsNotAllowedToBeMissing(Func<JsonElement, Result<BlogPost>> map)
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
            var blogPost = map(json);
            blogPost.Success.Should().BeFalse();
            blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Author.Age is required");
        }

        [Theory, MemberData(nameof(GetMappers))]
        public void AgeIsOptional(Func<JsonElement, Result<BlogPost>> map)
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
            var blogPost = map(json);
            blogPost.Should().BeSuccessful();
            blogPost.Value.Author.Age.Should().BeOfType<None<int>>();
        }

        [Theory, MemberData(nameof(GetMappers))]
        public void AuthorIsRequired(Func<JsonElement, Result<BlogPost>> map)
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {
                "Title": "Farting Unicorns",
                "IsDraft": true,
                "Category": "Horses",
                "Rating": 5
            }
            """);
            var blogPost = map(json);
            blogPost.Success.Should().BeFalse();
            blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Author is required");
        }

        [Theory, MemberData(nameof(GetMappers))]
        public void AuthorCannotBeNull(Func<JsonElement, Result<BlogPost>> map)
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
            var blogPost = map(json);
            blogPost.Success.Should().BeFalse();
            blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Author must have a value");
        }

        [Theory, MemberData(nameof(GetMappers))]
        public void AuthorIsWrongType(Func<JsonElement, Result<BlogPost>> map)
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
            var blogPost = map(json);
            blogPost.Success.Should().BeFalse();
            blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of $.Author has the wrong type. Expected Object, got Number");

        }
    }

    public partial class Optional
    {
        [CreateMapper]
        public partial class BlogPost
        {
            public string Title { get; set; }
            public bool IsDraft { get; set; }
            public Option<string> Category { get; set; }
            public Option<int> Rating { get; set; }
            public Option<Author> Author { get; set; }
        }
        public partial class Author
        {
            public string Name { get; set; }
            public Option<int> Age { get; set; }
        }
        public static IEnumerable<object[]> GetMappers =>
        [
            [(Func<JsonElement, Result<BlogPost>>)(x => Mapper.Map<BlogPost>(x))],
            [(Func<JsonElement, Result<BlogPost>>)(x => BlogPost.MapFromJson(x))]
        ];

        [Theory, MemberData(nameof(GetMappers))]
        public void Valid(Func<JsonElement, Result<BlogPost>> map)
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
            var blogPost = map(json);
            blogPost.Should().BeSuccessful();
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

        [Theory, MemberData(nameof(GetMappers))]
        public void AgeIsOptional_ButIsNotAllowedToBeMissing(Func<JsonElement, Result<BlogPost>> map)
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
            var blogPost = map(json);
            blogPost.Success.Should().BeFalse();
            blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Author.Age is required");
        }

        [Theory, MemberData(nameof(GetMappers))]
        public void AuthorCannotBeMissing(Func<JsonElement, Result<BlogPost>> map)
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {
                "Title": "Farting Unicorns",
                "IsDraft": true,
                "Category": "Horses",
                "Rating": 5
            }
            """);
            var blogPost = map(json);
            blogPost.Success.Should().BeFalse();
            blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Author is required");
        }

        [Theory, MemberData(nameof(GetMappers))]
        public void AuthorIsOptional(Func<JsonElement, Result<BlogPost>> map)
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
            var blogPost = map(json);
            blogPost.Should().BeSuccessful();
            blogPost.Value.Author.Should().BeOfType<None<Author>>();
        }

        [Theory, MemberData(nameof(GetMappers))]
        public void AuthorIsWrongType(Func<JsonElement, Result<BlogPost>> map)
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
            var blogPost = map(json);
            blogPost.Success.Should().BeFalse();
            blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of $.Author has the wrong type. Expected Object, got Number");

        }
    }

    public partial class Nullable
    {
        [CreateMapper]
        public partial class BlogPost
        {
            public Author? Author { get; set; }
        }
        public partial class Author
        {
            public string Name { get; set; }
            public Option<int> Age { get; set; }
        }

        public static IEnumerable<object[]> GetMappers =>
        [
            [(Func<JsonElement, Result<BlogPost>>)(x => Mapper.Map<BlogPost>(x))],
            [(Func<JsonElement, Result<BlogPost>>)(x => BlogPost.MapFromJson(x))]
        ];

        [Theory]
        [MemberData(nameof(GetMappers))]
        public void Valid(Func<JsonElement, Result<BlogPost>> map)
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {
                "Author": {
                    "Name": "John Doe",
                    "Age": 42
                }
            }
            """);
            var blogPost = map(json);
            blogPost.Should().BeSuccessful();
            blogPost.Value.Author.Should().NotBeNull();
            blogPost.Value.Author!.Name.Should().Be("John Doe");
            blogPost.Value.Author!.Age.Should().BeOfType<Some<int>>();
        }

        [Theory]
        [MemberData(nameof(GetMappers))]
        public void Missing_OK(Func<JsonElement, Result<BlogPost>> map)
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {
            }
            """);
            var blogPost = map(json);
            blogPost.Should().BeSuccessful();
            blogPost.Value.Author.Should().BeNull();
        }
    }

    public partial class NullableOptional
    {
        [CreateMapper]
        public partial class BlogPost
        {
            public Option<Author>? Author { get; set; }
        }

        public partial class Author
        {
            public string Name { get; set; }
            public Option<int> Age { get; set; }
        }

        public static IEnumerable<object[]> GetMappers =>
        [
            [(Func<JsonElement, Result<BlogPost>>)(x => Mapper.Map<BlogPost>(x))],
            [(Func<JsonElement, Result<BlogPost>>)(x => BlogPost.MapFromJson(x))]
        ];

        [Theory]
        [MemberData(nameof(GetMappers))]
        public void Valid(Func<JsonElement, Result<BlogPost>> map)
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {
                "Author": {
                    "Name": "John Doe",
                    "Age": 42
                }
            }
            """);
            var blogPost = map(json);
            blogPost.Should().BeSuccessful();
            blogPost.Value.Author.Should().NotBeNull();
            blogPost.Value.Author!.Should().BeOfType<Some<Author>>();
            var someAuthor = (blogPost.Value.Author as Some<Author>)!;
            someAuthor.Value.Name.Should().Be("John Doe");
            someAuthor.Value.Age.Should().BeOfType<Some<int>>();
        }

        [Theory]
        [MemberData(nameof(GetMappers))]
        public void Missing(Func<JsonElement, Result<BlogPost>> map)
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {
            }
            """);
            var blogPost = map(json);
            blogPost.Should().BeSuccessful();
            blogPost.Value.Author.Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(GetMappers))]
        public void Nulled(Func<JsonElement, Result<BlogPost>> map)
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {
                "Author": null
            }
            """);
            var blogPost = map(json);
            blogPost.Should().BeSuccessful();
            blogPost.Value.Author.Should().BeOfType<None<Author>>();
        }

    }

}
