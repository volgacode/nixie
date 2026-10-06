using System.Collections.Generic;

namespace Nixie.Lexer;

public static class Keywords
{
    private static readonly Dictionary<string, TokenType> Table = new()
    {
        { "bool", TokenType.Bool },
        { "int", TokenType.Int },
        { "string", TokenType.String },
        { "array", TokenType.Array },
        { "if", TokenType.If },
        { "else", TokenType.Else },
        { "while", TokenType.While },
        { "for", TokenType.For },
        { "void", TokenType.Void },
        { "return", TokenType.Return },
        { "struct", TokenType.Struct },
        { "true", TokenType.True },
        { "false", TokenType.False },
        { "null", TokenType.Null }
    };

    public static bool TryGetKeyword(string text, out TokenType tokenType)
    {
        return Table.TryGetValue(text, out tokenType);
    }
}