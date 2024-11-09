using DotNetThoughts.Results;
using System.Text.Json;

namespace FartingUnicorn.Generated;
public static partial class Mappers
{
    public static Result<FartingUnicorn.Benchmarks.UserProfile> MapToUserProfile(JsonElement jsonElement)
    {
        var result = new FartingUnicorn.Benchmarks.UserProfile();
        return Result<FartingUnicorn.Benchmarks.UserProfile>.Ok(result);
    }
}
