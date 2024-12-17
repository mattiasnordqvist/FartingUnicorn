using DotNetThoughts.Results;

using System.Text.Json;

namespace DotNetThoughts.FartingUnicorn
{
    public interface IConverter<T>
    {
        JsonValueKind ExpectedJsonValueKind { get; }

        Result<T> Convert(JsonElement jsonElement);
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ConverterAttribute<TConverter, T> : System.Attribute where TConverter : IConverter<T>
    {
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class CreateMapperAttribute : System.Attribute
    {
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class CreateMapperAttribute<T> : CreateMapperAttribute
    {
    }
}