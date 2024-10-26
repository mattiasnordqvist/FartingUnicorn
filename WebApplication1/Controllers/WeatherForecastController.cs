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
    [HttpPost]
    public async Task<Result<UserProfile>> Post([FromBody] JsonElement weatherForecast)
    {
        return Mapper.Map(weatherForecast);
    }
}
