﻿using DotNetThoughts.Results;
using System.Text.Json;

namespace FartingUnicorn.Generated;
public static partial class Mappers
{
    public static Result<WebApplication1.Controllers.WeatherForecastController.UserProfile> MapToUserProfile(JsonElement jsonElement)
    {
        var result = new WebApplication1.Controllers.WeatherForecastController.UserProfile();
        return Result<WebApplication1.Controllers.WeatherForecastController.UserProfile>.Ok(result);
    }
}
