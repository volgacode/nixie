namespace Nixie.Lexer;

public static class TokenTypeExtensions
{
    public static string ToRepresentation(this TokenType type) => type switch
    {
        TokenType.Bool => "bool",
        TokenType.Int => "int",
        TokenType.String => "string",
        TokenType.Array => "array",
        TokenType.If => "if",
        TokenType.Else => "else",
        TokenType.While => "while",
        TokenType.For => "for",
        TokenType.Void => "void",
        TokenType.Return => "return",
        TokenType.Struct => "struct",
        TokenType.True => "true",
        TokenType.False => "false",
        TokenType.Null => "null",
        TokenType.Assign => "=",
        TokenType.Equal => "==",
        TokenType.NotEqual => "!=",
        TokenType.Less => "<",
        TokenType.LessOrEqual => "<=",
        TokenType.Greater => ">",
        TokenType.GreaterOrEqual => ">=",
        TokenType.And => "&&",
        TokenType.Or => "||",
        TokenType.Not => "!",
        TokenType.Plus => "+",
        TokenType.Minus => "-",
        TokenType.Star => "*",
        TokenType.Slash => "/",
        TokenType.Dot => ".",
        TokenType.LBracket => "[",
        TokenType.RBracket => "]",
        TokenType.LParen => "(",
        TokenType.RParen => ")",
        TokenType.LBrace => "{",
        TokenType.RBrace => "}",
        TokenType.Comma => ",",
        TokenType.Semicolon => ";",
        _ => string.Empty
    };
}