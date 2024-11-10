using Namotion.Reflection;

using Xunit;

namespace FartingUnicorn.Tests;
public class DTests
{
    public class ClassWithProps
    {
        public int IntProp { get; set; }
        public int? NullableIntProp { get; set; }
        public Option<int> OptionIntProp { get; set; }
        public Option<int>? NullableOptionIntProp { get; set; }
        public string StringProp { get; set; }
        public string? NullableStringProp { get; set; }
        public Option<string> OptionStringProp { get; set; }
        public Option<string>? NullableOptionStringProp { get; set; }
    }

    public static IEnumerable<object[]> TestData()
    {
        yield return new object[] { typeof(int), null, false, typeof(int), false };
        yield return new object[] { typeof(int?), null, false, typeof(int), true };
        yield return new object[] { typeof(Option<int>), null, true, typeof(int), false };
        yield return new object[] { typeof(string), null, false, typeof(string), false };
        yield return new object[] { typeof(Option<string>), null, true, typeof(string), false };

        foreach(var prop in typeof(ClassWithProps).GetContextualProperties())
        {
            yield return new object[] { prop.PropertyInfo.PropertyType, prop, prop.Name.Contains("Option"), prop.Name.Contains("Int") ? typeof(int) : typeof(string), prop.Name.Contains("Nullable") };
        }
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void Test(Type input, ContextualPropertyInfo? cpi, bool isOptionExpected, Type baseTypeExpected, bool isNullableExpected)
    {
        
        var (isOption, type, isNullable) = Mapper.D(input, cpi);
        Assert.Equal(isOptionExpected, isOption);
        Assert.Equal(baseTypeExpected, type);
        Assert.Equal(isNullableExpected, isNullable);
    }
}
