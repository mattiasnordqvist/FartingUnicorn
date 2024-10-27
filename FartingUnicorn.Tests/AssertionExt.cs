using FluentAssertions;
using FluentAssertions.Primitives;

namespace FartingUnicorn.Tests;

public static class AssertionExt
{
    public static void BeNone<T>(this ObjectAssertions objectAssertions)
    {
        objectAssertions.BeOfType<None<T>>();
    }
    public static void BeSome<T>(this ObjectAssertions objectAssertions, T value)
    {
        objectAssertions.BeOfType<Some<T>>();
        var some = (Some<T>)objectAssertions.Subject!;
        some.Value.Should().Be(value);
    }

    public static void BeError<T>(this ObjectAssertions objectAssertions, T value)
    {
        objectAssertions.BeOfType<Some<T>>();
        var some = (Some<T>)objectAssertions.Subject!;
        some.Value.Should().Be(value);
    }
}
