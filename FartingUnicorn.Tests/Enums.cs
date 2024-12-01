using DotNetThoughts.FartingUnicorn;
using DotNetThoughts.Results;

using FluentAssertions;

using System.Text.Json;

using Xunit;

namespace FartingUnicorn.Tests;

public partial class Enums
{
    public partial class NotNullableNotOptionable
    {
        [CreateMapper]
        public partial class BlogPost
        {
            public string Title { get; set; }
            public BlogPostStatus Status { get; set; }
        }

        public enum BlogPostStatus { Draft, Published }

        public static IEnumerable<object[]> GetMappers =>
        [
            [(Func<JsonElement, MapperOptions, Result<BlogPost>>)((x, m) => Mapper.Map<BlogPost>(x, m, null))],
            [(Func<JsonElement, MapperOptions, Result<BlogPost>>)((x, m) => BlogPost.MapFromJson(x, m, null))]
        ];

        [Theory]
        [MemberData(nameof(GetMappers))]
        public void Valid(Func<JsonElement, MapperOptions, Result<BlogPost>> map)
        {
            var _mapperOptions = new MapperOptions();
            _mapperOptions.AddConverter(new EnumAsStringConverter());
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {"Title":"Farting Unicorns","Status":"Draft"}
            """);
            var blogPost = map(json, _mapperOptions);
            blogPost.Should().BeSuccessful();
            blogPost.Value.Title.Should().Be("Farting Unicorns");
            blogPost.Value.Status.Should().Be(BlogPostStatus.Draft);
        }

        [Theory]
        [MemberData(nameof(GetMappers))]
        public void InValid(Func<JsonElement, MapperOptions, Result<BlogPost>> map)
        {
            var _mapperOptions = new MapperOptions();
            _mapperOptions.AddConverter(new EnumAsStringConverter());
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {"Title":"Farting Unicorns","Status":"Snarf"}
            """);
            var blogPost = map(json, _mapperOptions);
            blogPost.Success.Should().BeFalse();
            blogPost.Errors.Should().ContainSingle().Which.Message.Should().Be("Failed to map $.Status: Snarf is not a valid BlogPostStatus. Valid BlogPostStatus alternatives: Draft, Published");
        }

        [Theory]
        [MemberData(nameof(GetMappers))]
        public void WontAllowNull(Func<JsonElement, MapperOptions, Result<BlogPost>> map)
        {
            var _mapperOptions = new MapperOptions();
            _mapperOptions.AddConverter(new EnumAsStringConverter());
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {"Title":"Farting Unicorns","Status":null}
            """);
            var blogPost = map(json, _mapperOptions);
            blogPost.Success.Should().BeFalse();
            blogPost.Errors.Should().ContainSingle().Which.Message.Should().Be("$.Status must have a value");
        }

        [Theory]
        [MemberData(nameof(GetMappers))]
        public void WontAllowMissing(Func<JsonElement, MapperOptions, Result<BlogPost>> map)
        {
            var _mapperOptions = new MapperOptions();
            _mapperOptions.AddConverter(new EnumAsStringConverter());
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {"Title":"Farting Unicorns"}
            """);
            var blogPost = map(json, _mapperOptions);
            blogPost.Success.Should().BeFalse();
            blogPost.Errors.Should().ContainSingle().Which.Message.Should().Be("$.Status is required");
        }
    }
}