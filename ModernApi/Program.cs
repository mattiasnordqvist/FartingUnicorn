
using DotNetThoughts.Results;

using FartingUnicorn;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

using System.Reflection;
using System.Text.Json;
using DotNetThoughts.FartingUnicorn.MinimalApi;
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

                schema.Nullable = false;
                foreach (var p in context.JsonTypeInfo.Properties)
                {
                    if (!p.IsGetNullable)
                    {
                        schema.Required.Add(p.Name);
                    }

                    if ((p.PropertyType.IsGenericType)
                        && p.PropertyType.GetGenericTypeDefinition() == typeof(Option<>))
                    {
                        schema.Properties.Where(x => x.Key == p.Name).First().Value.Nullable = true;
                        schema.Properties.Where(x => x.Key == p.Name).First().Value.Reference = new OpenApiReference
                        {
                            Id = p.PropertyType.GetGenericArguments()[0].Name,
                            Type = ReferenceType.Schema
                        };
                        //schema.Nullable = true;
                        //schema.Reference = new OpenApiReference
                        //{
                        //    Id = context.JsonTypeInfo.Type.GetGenericArguments()[0].Name,
                        //    Type = ReferenceType.Schema
                        //};
                    }
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

        app.UseExceptionHandler(app =>
        {
            app.Run(async c =>
            {
                var exceptionHandlerPathFeature =
                    c.Features.Get<IExceptionHandlerPathFeature>();

                if (exceptionHandlerPathFeature?.Error is ValueOrThrowException e)
                {
                    c.Response.StatusCode = StatusCodes.Status400BadRequest;
                    c.Response.ContentType = "application/json";
                    await c.Response.WriteAsJsonAsync(new
                    {
                        Success = false,
                        Errors = e.Errors.Select(x => new { x.Type, x.Message, Data = x.GetData() })
                    });
                }
            });
        });
        app.MapPost("/profile", (HttpContext httpContext, UserProfile profile) =>
        {
            return profile;
        }).Accepts<UserProfile>("application/json");
        app.MapPost("/profile2", (HttpContext httpContext, Pet pet) =>
        {
            return pet;
        });
        app.Run();
    }
}

[GenerateBindAsync]
public partial class UserProfile
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