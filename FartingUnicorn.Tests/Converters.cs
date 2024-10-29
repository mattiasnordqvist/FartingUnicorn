using DotNetThoughts.Results;

using FluentAssertions;

using System.Text.Json;

using Xunit;

using static FartingUnicorn.MapperOptions;

namespace FartingUnicorn.Tests;

public class Converters
{
    public class Id
    {
        public long Value { get; set; }

        public static Result<Id> FromInput(string candidate)
        {
            if (long.TryParse(candidate, out var value))
            {
                return Result<Id>.Ok(new Id { Value = value });
            }
            else
            {
                return Result<Id>.Error(new InvalidIdError(candidate));
            }
        }

        public record InvalidIdError(string candidate) : ErrorBase($"'{candidate}' is not a valid Id");
    }

    public class IdConverter : IConverter
    {
        public bool CanConvert(Type type)
        {
            return type == typeof(Id);
        }

        public JsonValueKind ExpectedJsonValueKind => JsonValueKind.String;

        public Result<object> Convert(Type clrType, JsonElement jsonElement, MapperOptions mapperOptions, string[] path)
        {
            return Id.FromInput(jsonElement.GetString()).Map(x => (object)x);
        }
    }

    public class BlogPost
    {
        public required Id Id { get; set; }
        public required string Title { get; set; }
    }

    [Fact]
    public void Valid()
    {
        var mapperOptions = new MapperOptions();
        mapperOptions.AddConverter(new IdConverter());
        var json = JsonSerializer.Deserialize<JsonElement>("""
        {
            "Id": "123456",
            "Title": "Farting Unicorns"
        }
        """);
        var blogPost = Mapper.Map<BlogPost>(json, mapperOptions);
        blogPost.Should().BeSuccessful();
        blogPost.Value.Id.Value.Should().Be(123456);
        blogPost.Value.Title.Should().Be("Farting Unicorns");
    }

    [Fact]
    public void InvalidId()
    {
        var mapperOptions = new MapperOptions();
        mapperOptions.AddConverter(new IdConverter());
        var json = JsonSerializer.Deserialize<JsonElement>("""
        {
            "Id": "Not a number",
            "Title": "Farting Unicorns"
        }
        """);
        var blogPost = Mapper.Map<BlogPost>(json, mapperOptions);
        blogPost.Success.Should().BeFalse();
        blogPost.Errors.Should().ContainSingle(e => e.Message == "Failed to map $.Id: 'Not a number' is not a valid Id");
    }
}
