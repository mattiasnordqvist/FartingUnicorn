using DotNetThoughts.Results;

using FluentAssertions;
using FluentAssertions.Collections;

namespace FartingUnicorn.Tests;

public static class IErrorCollectionAssertionExt
{
    public static void BeSingleError<T>(this GenericCollectionAssertions<IError> @this, string message)
    {
        @this.Subject!.Should().ContainSingle();
        @this.Subject!.Single().Should().BeOfType<T>();
        @this.Subject!.Single().Message.Should().Be(message);
    }
}
