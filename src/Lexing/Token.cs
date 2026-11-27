namespace Lexing;

public readonly record struct Token(
    TokenType Type,
    string Lexeme,
    SourcePosition Position,
    string? ErrorMessage = null);