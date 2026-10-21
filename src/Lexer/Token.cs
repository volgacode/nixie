namespace Nixie.Lexer;

public readonly record struct Token(
    TokenType Type,
    string Lexeme,
    SourcePosition Position,
    string? ErrorMessage = null
)
{
    public override string ToString() =>
        Type == TokenType.Error
            ? $"Error at {Position}: {ErrorMessage}"
            : $"{Type} ('{Lexeme}') at {Position}";
}