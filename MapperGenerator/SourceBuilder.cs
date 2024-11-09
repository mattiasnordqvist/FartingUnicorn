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

    public IDisposable Indent()
    {
        _indentLevel++;
        if (_indentLevel == _indentStrings.Count)
        {
            _indentStrings.Add(new string(' ', _indentSize * _indentLevel));
        }
        return new Unindent(() => _indentLevel--);
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

    private class Unindent : IDisposable
    {
        private Func<int> _unindent;

        public Unindent(Func<int> unindent)
        {
            _unindent = unindent;
        }

        public void Dispose()
        {
            _unindent();
        }
    }
}