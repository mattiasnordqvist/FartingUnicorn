using DotNetThoughts.Results;

using System.Text.Json;

using static FartingUnicorn.MapperOptions;

namespace FartingUnicorn;

public class EnumAsStringConverter : IConverter
{
    public bool CanConvert(Type type)
    {
        return type.IsEnum;
    }

    public JsonValueKind ExpectedJsonValueKind => JsonValueKind.String;

    public Result<object> Convert(Type clrType, JsonElement jsonElement, MapperOptions mapperOptions, string[] path)
    {
        if(Enum.TryParse(clrType, jsonElement.GetString(), out object? result))
        {
            return Result<object>.Ok(result);
        }
        else
        {
            return Result<object>.Error(new EnumValueMustExistError(clrType, jsonElement.GetString()));
        }
    }
}
public record EnumValueMustExistError : ErrorBase 
{
    private readonly Type _clrType;

    public string EnumName => _clrType.Name;
    public string[] ValidValues => Enum.GetValues(_clrType).Cast<object>().Select(x => x.ToString()).ToArray();
    public EnumValueMustExistError(Type clrType, string? candidate)
    {
        _clrType = clrType;

        Message = (candidate?.ToString() ?? "<null>") + $" is not a valid {EnumName}. Valid {EnumName} alternatives: " +
            string.Join(", ", ValidValues);
    }
}