using DotNetThoughts.Results;

using FluentAssertions;
using FluentAssertions.Execution;
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
public static class ResultExtensions
{
    public static ResultAssertions<T>  Should<T>(this Result<T> instance)
    {
        return new ResultAssertions<T>(instance);
    }
}

public class ResultAssertions<T> :
    ReferenceTypeAssertions<Result<T>, ResultAssertions<T>>
{
    public ResultAssertions(Result<T> instance)
        : base(instance)
    {
    }

    protected override string Identifier => "directory";

    [CustomAssertion]
    public AndConstraint<ResultAssertions<T>> BeSuccessful(
        string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.Success)
            .FailWith($"""
                Expected successful result, but found the following errors: 
                {string.Join(Environment.NewLine, Subject.Errors.Select(x => $"- {x.Type}: {x.Message}"))}
                """);

        return new AndConstraint<ResultAssertions<T>>(this);
    }
}