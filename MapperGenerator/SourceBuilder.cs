using Microsoft.CodeAnalysis.Text;

using System.Text;

namespace MapperGenerator;

public class SourceBuilder
{
    private int _indentLevel;
    private readonly int _indentSize;
    private readonly List<string> _indentStrings;
    private readonly StringBuilder _stringBuilder;

    public SourceBuilder()
    {
        _indentLevel = 0;
        _indentSize = 4;
        _indentStrings = new List<string> { "" };
        _stringBuilder = new StringBuilder();
    }
    public void AppendLine(string line)
    {
        if (_stringBuilder != null)
        {
            _stringBuilder
                .Append(_indentStrings[_indentLevel])
                .AppendLine(line);
        }
    }

    public IDisposable CodeBlock()
    {
        AppendLine("{");
        var indent = Indent();

        return new DisposeFunc(() =>
        {
            indent.Dispose();
            AppendLine("}");
        });

    }
    public IDisposable Indent()
    {
        _indentLevel++;
        if (_indentLevel == _indentStrings.Count)
        {
            _indentStrings.Add(new string(' ', _indentSize * _indentLevel));
        }
        return new DisposeFunc(() => _indentLevel--);
    }

    public override string? ToString()
    {
        return _stringBuilder.ToString();
    }

    public SourceText ToSourceText()
    {
        return SourceText.From(_stringBuilder.ToString(), Encoding.UTF8);
    }

    internal void AppendLine()
    {
        _stringBuilder.AppendLine();
    }
    private class DisposeFunc : IDisposable
    {
        private Action _endBlock;
        public DisposeFunc(Action endBlock)
        {
            _endBlock = endBlock;
        }
        public void Dispose()
        {
            _endBlock();
        }
    }
   
}