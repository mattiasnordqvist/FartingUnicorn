﻿using DotNetThoughts.FartingUnicorn;
using DotNetThoughts.Results;

using FluentAssertions;

using System.Text.Json;

using Xunit;

namespace FartingUnicorn.Tests;
public partial class Objects
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
            blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Author.Age is required");
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
            blogPost.Should().BeSuccessful();
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
            blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Author is required");
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
            blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Author must have a value");
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
            blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of $.Author has the wrong type. Expected Object, got Number");

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
            blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Author.Age is required");
        }

        [Fact]
        public void AuthorCannotBeMissing()
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
            blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Author is required");
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
            blogPost.Should().BeSuccessful();
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
            [(Func<JsonElement, Result<BlogPost>>)(x => Generated.Mappers.MapToFartingUnicorn_Tests_Objects_Nullable_BlogPost(x))]
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
            blogPost.Should().BeSuccessful();
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
            blogPost.Should().BeSuccessful();
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
            blogPost.Should().BeSuccessful();
            blogPost.Value.Author.Should().BeOfType<None<Author>>();
        }

    }

}
