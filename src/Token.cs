namespace Nixie.Lexer;

public readonly record struct Token(
    TokenType Type,
    string Lexeme,
    SourceLocation Location,
    string ErrorMessage = ""
)
{
    public override string ToString() =>
        Type == TokenType.Error
            ? $"Error at {Location}: {ErrorMessage}"
            : $"{Type} ('{Lexeme}') at {Location}";
}