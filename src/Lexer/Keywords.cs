namespace Nixie.Lexer;

public static class Keywords
{
    private static readonly Dictionary<string, TokenType> Table = new()
    {
        { "bool", TokenType.Bool },
        { "int", TokenType.Int },
        { "if", TokenType.If },
        { "else", TokenType.Else },
        { "while", TokenType.While },
        { "for", TokenType.For },
        { "void", TokenType.Void },
        { "return", TokenType.Return },
        { "true", TokenType.True },
        { "false", TokenType.False },
    };

    public static bool TryGetKeyword(string text, out TokenType tokenType)
    {
        return Table.TryGetValue(text, out tokenType);
    }
}