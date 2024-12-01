using DotNetThoughts.Results;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace FartingUnicorn;

public interface IConverter
{
    bool CanConvert(Type clrType);

    JsonValueKind ExpectedJsonValueKind { get; }

    Result<object> Convert(Type clrType, JsonElement jsonElement, MapperOptions mapperOptions, string[] path);
}

public class MapperOptions
{
    

    public List<IConverter> _converters = [];

    public void AddConverter(IConverter converter)
    {
        _converters.Add(converter);
    }

    public bool TryGetConverter(Type type, [NotNullWhen(true)] out IConverter? converter)
    {
        converter = _converters.FirstOrDefault(x => x.CanConvert(type));
        return converter != null;
    }
}
