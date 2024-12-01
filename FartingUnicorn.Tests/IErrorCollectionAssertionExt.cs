using DotNetThoughts.Results;

using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Execution;

namespace FartingUnicorn.Tests;

public static class IErrorCollectionAssertionExt
{
    public static void BeSingleError<T>(this GenericCollectionAssertions<IError> @this, string message)
    {
        using(new AssertionScope())
        {
            @this.Subject!.Should().ContainSingle();
            @this.Subject!.Single().Should().BeOfType<T>();
            @this.Subject!.Single().Message.Should().Be(message);
        }
    }

    public static void BeErrors(this GenericCollectionAssertions<IError> @this, (Type, string)[] messages)
    {
        using (new AssertionScope())
        {
            @this.Subject!.Should().HaveCount(messages.Length);
            
            for (int i = 0; i < messages.Length; i++)
            {
                @this.Subject!.Should().ContainSingle(e => e.GetType() == messages[i].Item1 && e.Message == messages[i].Item2);
            }
        }
    }
}
