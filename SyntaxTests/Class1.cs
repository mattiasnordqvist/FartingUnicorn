using Microsoft.CodeAnalysis.CSharp;

using Xunit;

namespace SyntaxTests;

public class Class1
{
    [Fact]
    public void Test()
    {
        var parsed = SyntaxFactory.ParseCompilationUnit("""
            namespace ParseTests;

            public class BlogPost
            {
                public Author? Author { get; set; }
            }
            public class Author
            {
                public string Name { get; set; }
                public Option<int> Age { get; set; }
            }
            """);
    }
}
