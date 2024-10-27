using DotNetThoughts.Results;

using System.Text.Json;

namespace WebApplication1.Controllers;

public static class FartingControllerExtensions
{
    public static async Task<Result<T>> MapBody<T>(this HttpRequest request)
    {
        using var json = await JsonDocument.ParseAsync(request.Body);
        var rootElement = json.RootElement;
        return FartingUnicorn.Mapper.Map<T>(rootElement);
    }
}
