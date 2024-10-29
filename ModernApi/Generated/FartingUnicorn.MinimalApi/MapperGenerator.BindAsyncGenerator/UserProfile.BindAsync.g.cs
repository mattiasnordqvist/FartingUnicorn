/* auto-generated */
using DotNetThoughts.Results;
using FartingUnicorn;

using System.Reflection;
using System.Text.Json;

namespace ModernApi;
public partial class UserProfile {
    public static async ValueTask<UserProfile> BindAsync(HttpContext context, ParameterInfo parameter) {
        if (!context.Request.HasJsonContentType())
        {
            throw new BadHttpRequestException(
                "Request content type was not a recognized JSON content type.",
                StatusCodes.Status415UnsupportedMediaType);
        }

        using var json = await JsonDocument.ParseAsync(context.Request.Body);
        var rootElement = json.RootElement;
        return Mapper.Map<UserProfile>(rootElement).ValueOrThrow();
    }
}
