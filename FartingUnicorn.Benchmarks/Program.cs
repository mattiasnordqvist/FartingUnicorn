using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

using System.Text;
using System.Text.Json;
using FartingUnicorn;
using DotNetThoughts.Results;
namespace FartingUnicorn.Benchmarks;

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<SerializationBenchmarks>();
    }
}

[DotNetThoughts.FartingUnicorn.CreateMapper]
public partial class UserProfile
{
    public string Name { get; set; }
    //public int Age { get; set; }
    //public bool IsSubscribed { get; set; }
    //public string[] Courses { get; set; }
    //public Pet Pet { get; set; }

    //public bool? IsGay { get; set; }
    //public Pet? FavoritePet { get; set; }
}

public class Pet
{
    public string Name { get; set; }
    public string Type { get; set; }
}

[MemoryDiagnoser]
public class SerializationBenchmarks
{
   

    private static string _json = """
        {   
            "Name": "John Doe"
        }
        """;

    private static Stream _jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(_json));

    [Benchmark]
    public async Task<UserProfile> ClassicDeserializeToT()
    {
        _jsonStream.Seek(0, SeekOrigin.Begin);
        return (await JsonSerializer.DeserializeAsync<UserProfile>(_jsonStream))!;
    }

    [Benchmark]
    public async Task<UserProfile> FartingDeserialization()
    {
        _jsonStream.Seek(0, SeekOrigin.Begin);
        using var json = await JsonDocument.ParseAsync(_jsonStream);
        return Mapper.Map<UserProfile>(json.RootElement).ValueOrThrow();
    }

    [Benchmark]
    public async Task<UserProfile> FartingGeneratedDeserialization()
    {
        _jsonStream.Seek(0, SeekOrigin.Begin);
        using var json = await JsonDocument.ParseAsync(_jsonStream);
        return UserProfile.MapFromJson(json.RootElement).ValueOrThrow();
    }
}
