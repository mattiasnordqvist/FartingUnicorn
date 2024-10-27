using DotNetThoughts.Results;

using Microsoft.AspNetCore.Mvc;

using System.Text.Json;

namespace WebApplication1.Controllers;
[ApiController]
[Route("[controller]")]
public partial class WeatherForecastController : ControllerBase
{

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
