using DotNetThoughts.Results.Json;

namespace WebApplication1;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers(options =>
        {
            options.InputFormatters.Insert(0, new FartingFormatter(new FartingUnicorn.MapperOptions()));
        })
            .AddJsonOptions(configure =>
            {
                configure.JsonSerializerOptions.Converters.Add(new JsonConverterFactoryForResultOfT());
            }) ;

        var app = builder.Build();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
