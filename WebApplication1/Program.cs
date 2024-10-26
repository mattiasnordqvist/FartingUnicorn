using DotNetThoughts.Results.Json;

using System.Text.Json;

namespace WebApplication1;

public class Program
{
    public static void Main(string[] args)
    {


        var builder = WebApplication.CreateBuilder(args);


        builder.Services.AddControllers()
            .AddJsonOptions(configure =>
            {
                configure.JsonSerializerOptions.Converters.Add(new JsonConverterFactoryForResultOfT());
            }) ;

        var app = builder.Build();


        // Configure the HTTP request pipeline.

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
