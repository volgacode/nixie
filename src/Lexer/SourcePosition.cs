namespace Nixie.Lexer;

/// <summary>
/// Представляет позицию в исходном коде (строка и столбец).
/// </summary>
public readonly record struct SourcePosition(int Line = 1, int Column = 1)
{
    public override string ToString() => $"({Line}:{Column})";
}