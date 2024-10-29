using FluentAssertions;

using System.Text.Json;

using Xunit;

namespace FartingUnicorn.Tests;

public class Enums
{
    public class NotNullableNotOptionable
    {
        public class BlogPost
        {
            public string Title { get; set; }
            public BlogPostStatus Status { get; set; }
        }

        public enum BlogPostStatus { Draft, Published }

        [Fact]
        public void Valid()
        {
            var _mapperOptions = new MapperOptions();
            _mapperOptions.AddConverter(new EnumAsStringConverter());
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {"Title":"Farting Unicorns","Status":"Draft"}
            """);
            var blogPost = Mapper.Map<BlogPost>(json, mapperOptions: _mapperOptions);
            blogPost.Should().BeSuccessful();
            blogPost.Value.Title.Should().Be("Farting Unicorns");
            blogPost.Value.Status.Should().Be(BlogPostStatus.Draft);
        }
        [Fact]
        public void InValid()
        {
            var _mapperOptions = new MapperOptions();
            _mapperOptions.AddConverter(new EnumAsStringConverter());
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {"Title":"Farting Unicorns","Status":"Snarf"}
            """);
            var blogPost = Mapper.Map<BlogPost>(json, mapperOptions: _mapperOptions);
            blogPost.Success.Should().BeFalse();
            blogPost.Errors.Should().ContainSingle().Which.Message.Should().Be("Failed to map $.Status: Snarf is not a valid BlogPostStatus. Valid BlogPostStatus alternatives: Draft, Published");
        }

        [Fact]
        public void WontAllowNull()
        {
            var _mapperOptions = new MapperOptions();
            _mapperOptions.AddConverter(new EnumAsStringConverter());
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {"Title":"Farting Unicorns","Status":null}
            """);
            var blogPost = Mapper.Map<BlogPost>(json, mapperOptions: _mapperOptions);
            blogPost.Success.Should().BeFalse();
            blogPost.Errors.Should().ContainSingle().Which.Message.Should().Be("$.Status must have a value");
        }

        [Fact]
        public void WontAllowMissing()
        {
            var _mapperOptions = new MapperOptions();
            _mapperOptions.AddConverter(new EnumAsStringConverter());
            var json = JsonSerializer.Deserialize<JsonElement>("""
            {"Title":"Farting Unicorns"}
            """);
            var blogPost = Mapper.Map<BlogPost>(json, mapperOptions: _mapperOptions);
            blogPost.Success.Should().BeFalse();
            blogPost.Errors.Should().ContainSingle().Which.Message.Should().Be("$.Status is required");
        }
    }
}