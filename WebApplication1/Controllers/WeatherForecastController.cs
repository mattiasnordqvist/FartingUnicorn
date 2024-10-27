using DotNetThoughts.Results;

using Microsoft.AspNetCore.Mvc;

using System.Text.Json;

namespace WebApplication1.Controllers;
[ApiController]
[Route("[controller]")]
public partial class WeatherForecastController : ControllerBase
{
    [HttpPost("blog")]
    public Task Post([FromBody] BlogPost blogPost)
    {
        return Task.CompletedTask;
    }

    [HttpPost("profile1")]
    public async Task<Result<UserProfile>> Post([FromBody] JsonElement userProfile)
    {
        return FartingUnicorn.Mapper.Map<UserProfile>(userProfile);
    }

    [HttpPost("profile2")]
    public async Task<Result<UserProfile>> Post()
    {
        return await Request.MapBody<UserProfile>();
    }

}

public static class FartingControllerExtensions
{
    public static async Task<Result<T>> MapBody<T>(this HttpRequest request)
        where T : new()
    {
        using var json = await JsonDocument.ParseAsync(request.Body);
        var rootElement = json.RootElement;
        return FartingUnicorn.Mapper.Map<T>(rootElement);
    }
}
