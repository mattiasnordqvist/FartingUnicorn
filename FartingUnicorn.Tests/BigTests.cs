using FluentAssertions;

using System.Text.Json;

using Xunit;

namespace FartingUnicorn.Tests;

public class BigTests
{
    public class Put
    {
        public class BlogPost
        {
            public string Title { get; set; }
            public bool IsDraft { get; set; }
            public Option<string> Category { get; set; }
            public Option<int> Rating { get; set; }
            public Author Author { get; set; }
            public Comment[] Comments { get; set; }
        }

        public class Author
        {
            public string Name { get; set; }
            public Option<int> Age { get; set; }
        }
        public class Comment
        {
            public string Text { get; set; }
            public int Upvotes { get; set; }
            public Option<string> Contact { get; set; }
        }

        [Fact]
        public void AllAtOnce_EverythingIsValid()
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
                },
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
        }

        [Fact]
        public void AllAtOnce_EverythingIsValid_ExceptOneComment()
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
                },
                "Comments": [
                    {
                        "Text": "First!",
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
            blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Comments.1.Contact is required");
        }

        [Fact]
        public void AllAtOnce_AboutHalfIsInvalid()
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {
                "Title": "Farting Unicorns",
                "IsDraft": null,
                "Category": 123,
                "Author": {
                    "Name": "John Doe",
                    "Age": 42
                },
                "Comments": [
                    {
                        "Text": "First!",
                        "Upvotes": 5,
                        "Contact": "John Doe"
                    },
                    {
                        "Text": "Second!",
                        "Upvotes": 3
                    },
                    {
                        "Text": "Third!",
                        "Contact": "John Doe"
                    }
                ]
            }
            """);
            var blogPost = Mapper.Map<BlogPost>(json);
            blogPost.Success.Should().BeFalse();
            blogPost.Errors.Should().HaveCount(5);
            blogPost.Errors.Should().ContainSingle(e => e.Message == "$.IsDraft must have a value");
            blogPost.Errors.Should().ContainSingle(e => e.Message == "Value of $.Category has the wrong type. Expected String, got Number");
            blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Rating is required");
            blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Comments.1.Contact is required");
            blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Comments.2.Upvotes is required");
        }
    }
}