namespace Nixie.Lexer;

public readonly record struct SourceLocation(int Line = 1, int Column = 1)
{
    public override string ToString() => $"({Line}:{Column})";
}