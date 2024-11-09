using DotNetThoughts.Results;
using System.Text.Json;

namespace FartingUnicorn.Generated;
public static partial class Mappers
{
    public static Result<FartingUnicorn.Tests.SingleField.ReferenceType.NonNullableNonOptional_Tests.BlogPost> MapToBlogPost(JsonElement jsonElement)
    {
        var result = new FartingUnicorn.Tests.SingleField.ReferenceType.NonNullableNonOptional_Tests.BlogPost();
        return Result<FartingUnicorn.Tests.SingleField.ReferenceType.NonNullableNonOptional_Tests.BlogPost>.Ok(result);
    }
}
