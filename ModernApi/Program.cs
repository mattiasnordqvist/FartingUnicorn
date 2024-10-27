
using FartingUnicorn;

using System.Reflection;
using System.Text.Json;

namespace ModernApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization();
        builder.Services.AddOpenApi(options =>
        {
            options.AddSchemaTransformer((schema, context, cancellationToken) =>
            {
                if (context.JsonTypeInfo.Type.IsGenericType 
                &&  context.JsonTypeInfo.Type.GetGenericTypeDefinition() == typeof(Option<>))
                {
                    schema.Nullable = true;
                    schema.Type =
                    schema.Format = "decimal";
                }
                return Task.CompletedTask;
            });
        });

        var app = builder.Build();
        app.MapOpenApi();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "Modern API");
        });

        app.UseAuthorization();


        app.MapPost("/profile", (HttpContext httpContext, Fart<UserProfile> profile) =>
        {
            return profile.Value;
        }).Accepts<UserProfile>("application/json")
        .WithOpenApi();
        

        app.Run();
    }
}


public class UserProfile
{
    public string Name { get; set; }
    public int Age { get; set; }
    public bool IsSubscribed { get; set; }
    public string[] Courses { get; set; }
    public Option<Pet> Pet { get; set; }

    public bool? IsGay { get; set; }
    public Pet? FavoritePet { get; set; }
}

public class Pet
{
    public string Name { get; set; }
    public string Type { get; set; }
}

public class Fart<TModel>
{
    public Fart(TModel? value)
    {
        Value = value;
    }

    public TModel? Value { get; }

    public static async ValueTask<Fart<TModel>?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        if (!context.Request.HasJsonContentType())
        {
            throw new BadHttpRequestException(
                "Request content type was not a recognized JSON content type.",
                StatusCodes.Status415UnsupportedMediaType);
        }

        using var json = await JsonDocument.ParseAsync(context.Request.Body);
        var rootElement = json.RootElement;
        return new Fart<TModel>(Mapper.Map<TModel>(rootElement).ValueOrThrow());
    }
}