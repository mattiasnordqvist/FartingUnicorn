﻿using DotNetThoughts.Results;

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
}
