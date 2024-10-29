using FluentAssertions;

using System.Text.Json;

using Xunit;

namespace FartingUnicorn.Tests;

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
            blogPost.Should().BeSuccessful();
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
            blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Comments.0.Text is required");
            blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Comments.1.Contact is required");

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
            blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Comments is required");
        }
    }

    public class Optional
    {
        public class BlogPost
        {
            public Option<Comment[]> Comments { get; set; }
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
            blogPost.Should().BeSuccessful();
            blogPost.Value.Comments.Should().BeOfType<Some<Comment[]>>();
            var someComments = (blogPost.Value.Comments as Some<Comment[]>)!;
            someComments.Value.Should().HaveCount(2);
            someComments.Value[0].Text.Should().Be("First!");
            someComments.Value[0].Upvotes.Should().Be(5);
            someComments.Value[0].Contact.Should().BeOfType<Some<string>>();
            var someAuthor = (someComments.Value[0].Contact as Some<string>)!;
            someAuthor.Value.Should().Be("John Doe");
            someComments.Value[1].Text.Should().Be("Second!");
            someComments.Value[1].Upvotes.Should().Be(3);
            someComments.Value[1].Contact.Should().BeOfType<None<string>>();
            var someAuthor2 = (someComments.Value[1].Contact as None<string>)!;
        }

        [Fact]
        public void MissingArray_NotOk()
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
                    {
                    }
                    """);
            var blogPost = Mapper.Map<BlogPost>(json);
            blogPost.Success.Should().BeFalse();
            blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Comments is required");
        }

        [Fact]
        public void ArrayNulled_OK()
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
                    {
                        "Comments": null
                    }
                    """);
            var blogPost = Mapper.Map<BlogPost>(json);
            blogPost.Should().BeSuccessful();
            blogPost.Value.Comments.Should().BeOfType<None<Comment[]>>();
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
            blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Comments.0.Text is required");
            blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Comments.1.Contact is required");

        }

    }

    public class Nullable
    {
        public class BlogPost
        {
            public Comment[]? Comments { get; set; }
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
            blogPost.Should().BeSuccessful();
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
        public void MissingArray_OK()
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
                    {
                    }
                    """);
            var blogPost = Mapper.Map<BlogPost>(json);
            blogPost.Should().BeSuccessful();
            blogPost.Value.Comments.Should().BeNull();
        }

        [Fact]
        public void NulledArray_NotOk()
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
                    {
                        "Comments": null
                    }
                    """);
            var blogPost = Mapper.Map<BlogPost>(json);
            blogPost.Success.Should().BeFalse();
            blogPost.Errors.Should().ContainSingle(e => e.Message == "$.Comments must have a value");
        }
    }

    public class NullableOptional
    {
        public class BlogPost
        {
            public Option<Comment[]>? Comments { get; set; }
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
            blogPost.Should().BeSuccessful();
            blogPost.Value.Comments.Should().BeOfType<Some<Comment[]>>();
            var someComments = (blogPost.Value.Comments as Some<Comment[]>)!;
            someComments.Value.Should().HaveCount(2);
            someComments.Value[0].Text.Should().Be("First!");
            someComments.Value[0].Upvotes.Should().Be(5);
            someComments.Value[0].Contact.Should().BeOfType<Some<string>>();
            var someAuthor = (someComments.Value[0].Contact as Some<string>)!;
            someAuthor.Value.Should().Be("John Doe");
            someComments.Value[1].Text.Should().Be("Second!");
            someComments.Value[1].Upvotes.Should().Be(3);
            someComments.Value[1].Contact.Should().BeOfType<None<string>>();
            var someAuthor2 = (someComments.Value[1].Contact as None<string>)!;
        }

        [Fact]
        public void MissingArray_OK()
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
                    {
                    }
                    """);
            var blogPost = Mapper.Map<BlogPost>(json);
            blogPost.Should().BeSuccessful();
            blogPost.Value.Comments.Should().BeNull();
        }

        [Fact]
        public void NulledArray_Ok()
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
                    {
                        "Comments": null
                    }
                    """);
            var blogPost = Mapper.Map<BlogPost>(json);
            blogPost.Should().BeSuccessful();
            blogPost.Value.Comments.Should().BeOfType<None<Comment[]>>();
        }
    }

    public class PrimitiveArrays
    {
        public class BlogPost
        {
            public string[] Categories { get; set; }
        }

        [Fact]
        public void Valid()
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {
                "Categories": ["Farts", "Unicorns", "Horses", "Rainbows"]
            }
            """);
            var blogPost = Mapper.Map<BlogPost>(json);
            blogPost.Should().BeSuccessful();
            blogPost.Value.Categories.Should().HaveCount(4);
            blogPost.Value.Categories[0].Should().Be("Farts");
            blogPost.Value.Categories[1].Should().Be("Unicorns");
            blogPost.Value.Categories[2].Should().Be("Horses");
            blogPost.Value.Categories[3].Should().Be("Rainbows");
        }
    }

    public class ArrayOfArrayOrArrays
    {
        [Fact]
        public void Valid()
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
            [
                [["Farts","Warts"], ["Unicorns"]],
                [["Horses","Brows"], ["Rainbows","Llamacorns"]]
            ]
            """);
            var lists = Mapper.MapElement<string[][][]>(json);
            lists.Should().BeSuccessful();
        }

        [Fact]
        public void InValid()
        {
            var json = JsonSerializer.Deserialize<JsonElement>("""
            [
                [["Farts",null], ["Unicorns"]],
                [[null,"Brows"], null]
            ]
            """);
            var lists = Mapper.MapElement<string[][][]>(json);
            lists.Success.Should().BeFalse();
            lists.Errors.Should().HaveCount(3);
            lists.Errors.Should().ContainSingle(e => e.Message == "$.0.0.1 must have a value");
            lists.Errors.Should().ContainSingle(e => e.Message == "$.1.0.0 must have a value");
            lists.Errors.Should().ContainSingle(e => e.Message == "$.1.1 must have a value");

        }
    }
}
