using DotNetThoughts.Results.Json;

using Microsoft.AspNetCore.Mvc.Formatters;

using System.Text;
using System.Text.Json;

using static FartingUnicorn.Mapper;

namespace WebApplication1;

public class Program
{
    public static void Main(string[] args)
    {


        var builder = WebApplication.CreateBuilder(args);


        builder.Services.AddControllers(options =>
        {
            options.InputFormatters.Insert(0, new FartingFormatter());
        })
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

internal class FartingFormatter : TextInputFormatter, IInputFormatter
{
    public FartingFormatter()
    {
        SupportedMediaTypes.Add("application/json");
        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
    }
    override public bool CanRead(InputFormatterContext context)
    {
        return true;
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
    {
        using var json = await JsonDocument.ParseAsync(context.HttpContext.Request.Body);
        var rootElement = json.RootElement;
        var mapped = FartingUnicorn.Mapper.MapElement(context.ModelType, rootElement, null, [context.Metadata.Name ?? "$"]);
        if (mapped.Success)
        {
            return await InputFormatterResult.SuccessAsync(mapped.Value);
        }
        else
        {
            foreach(var e in mapped.Errors)
            {
                if(e is FartingUnicornErrorBase fartingUnicornErrorBase)
                {
                    context.ModelState.AddModelError(string.Join(".", fartingUnicornErrorBase.Path), fartingUnicornErrorBase.Message);
                }
            }
            
            return await InputFormatterResult.FailureAsync();
        }
    }
}