using FartingUnicorn;

using Microsoft.AspNetCore.Mvc.Formatters;

using System.Text;
using System.Text.Json;

using static FartingUnicorn.Mapper;

namespace WebApplication1;

internal class FartingFormatter : TextInputFormatter, IInputFormatter
{
    private readonly MapperOptions _mapperOptions;

    public FartingFormatter(MapperOptions mapperOptions)
    {
        SupportedMediaTypes.Add("application/json");
        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
        _mapperOptions = mapperOptions;
    }
    override public bool CanRead(InputFormatterContext context)
    {
        return true;
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
    {
        using var json = await JsonDocument.ParseAsync(context.HttpContext.Request.Body);
        var rootElement = json.RootElement;
        var mapped = MapElement(context.ModelType, rootElement, _mapperOptions, [context.Metadata.Name ?? "$"]);
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