using Xunit;

namespace Nixie.Lexer.Tests;

public class LexerTests
{
    public static TheoryData<string, TokenType> SingleTokenData() => new()
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
        { "null", TokenType.Null },
        { "=", TokenType.Assign },
        { "==", TokenType.Equal },
        { "!=", TokenType.NotEqual },
        { "<", TokenType.Less },
        { "<=", TokenType.LessOrEqual },
        { ">", TokenType.Greater },
        { ">=", TokenType.GreaterOrEqual },
        { "&&", TokenType.And },
        { "||", TokenType.Or },
        { "!", TokenType.Not },
        { "+", TokenType.Plus },
        { "-", TokenType.Minus },
        { "*", TokenType.Star },
        { "/", TokenType.Slash },
        { ".", TokenType.Dot },
        { "[", TokenType.LBracket },
        { "]", TokenType.RBracket },
        { "(", TokenType.LParen },
        { ")", TokenType.RParen },
        { "{", TokenType.LBrace },
        { "}", TokenType.RBrace },
        { ",", TokenType.Comma },
        { ";", TokenType.Semicolon },
        { "007", TokenType.Number },
        { "42", TokenType.Number },
        { "\"\"", TokenType.StringValue },
        { "\"hello\"", TokenType.StringValue },
        { "\"a\\\"b\\\\c\\nd\\te\\rf\"", TokenType.StringValue },
    };

    public static TheoryData<string> IdentifiersData() => new()
    {
        "x",
        "_",
        "_var",
        "test123",
        "ifVar",
        "str_print",
        "IF",
        "Int",
        "NULL",
    };

    public static TheoryData<string, TokenType[]> TokenSequenceData() => new()
    {
        { string.Empty, [TokenType.Eof] },
        { " \t\r\n\f  int", [TokenType.Int, TokenType.Eof] },
        { "// comment", [TokenType.Eof] },
        { "// c\nint x;", [TokenType.Int, TokenType.Identifier, TokenType.Semicolon, TokenType.Eof] },
        { "/* c\nd */ int /* e */ x;", [TokenType.Int, TokenType.Identifier, TokenType.Semicolon, TokenType.Eof] },
        { "/* c */", [TokenType.Eof] },
        { "/**/int", [TokenType.Int, TokenType.Eof] },
        { "a/*c*/b", [TokenType.Identifier, TokenType.Identifier, TokenType.Eof] },
        { "/* /* */ */", [TokenType.Star, TokenType.Slash, TokenType.Eof] },
        { "a==b", [TokenType.Identifier, TokenType.Equal, TokenType.Identifier, TokenType.Eof] },
        { "a!=b", [TokenType.Identifier, TokenType.NotEqual, TokenType.Identifier, TokenType.Eof] },
        { "a<=b", [TokenType.Identifier, TokenType.LessOrEqual, TokenType.Identifier, TokenType.Eof] },
        { "a>=b", [TokenType.Identifier, TokenType.GreaterOrEqual, TokenType.Identifier, TokenType.Eof] },
        { "a&&b", [TokenType.Identifier, TokenType.And, TokenType.Identifier, TokenType.Eof] },
        { "a||b", [TokenType.Identifier, TokenType.Or, TokenType.Identifier, TokenType.Eof] },
        { "a=!b", [TokenType.Identifier, TokenType.Assign, TokenType.Not, TokenType.Identifier, TokenType.Eof] },
        { "a/b", [TokenType.Identifier, TokenType.Slash, TokenType.Identifier, TokenType.Eof] },
        { "a!==b", [TokenType.Identifier, TokenType.NotEqual, TokenType.Assign, TokenType.Identifier, TokenType.Eof] },
        { "if(x)", [TokenType.If, TokenType.LParen, TokenType.Identifier, TokenType.RParen, TokenType.Eof] },
        { "else{", [TokenType.Else, TokenType.LBrace, TokenType.Eof] },
        { "arr[0].x", [TokenType.Identifier, TokenType.LBracket, TokenType.Number, TokenType.RBracket, TokenType.Dot, TokenType.Identifier, TokenType.Eof] },
        { "int main() { return 0; }", [TokenType.Int, TokenType.Identifier, TokenType.LParen, TokenType.RParen, TokenType.LBrace, TokenType.Return, TokenType.Number, TokenType.Semicolon, TokenType.RBrace, TokenType.Eof] },
    };

    public static TheoryData<string, TokenType[]> InvalidCharactersData() => new()
    {
        { "@", [TokenType.Error] },
        { "a @ b", [TokenType.Identifier, TokenType.Error] },
        { "&", [TokenType.Error] },
        { "a | b", [TokenType.Identifier, TokenType.Error] },
        { "abcй", [TokenType.Identifier, TokenType.Error] },
        { "тест", [TokenType.Error] },
        { "#", [TokenType.Error] },
        { "$", [TokenType.Error] },
        { "~", [TokenType.Error] },
        { "?", [TokenType.Error] },
        { ":", [TokenType.Error] },
        { "'", [TokenType.Error] },
        { "\\", [TokenType.Error] },
        { "^", [TokenType.Error] },
        { "%", [TokenType.Error] },
        { "\v", [TokenType.Error] },
        { "\u00A0", [TokenType.Error] },
    };

    public static TheoryData<string, TokenType[]> InvalidLiteralsAndCommentsData() => new()
    {
        { "\"abc", [TokenType.Error] },
        { "\"abc\\", [TokenType.Error] },
        { "\"a\\xb\"", [TokenType.Error] },
        { "\"a\nb\"", [TokenType.Error] },
        { "\"a\rb\"", [TokenType.Error] },
        { "/*", [TokenType.Error] },
        { "/*/", [TokenType.Error] },
        { "int x; /* oops", [TokenType.Int, TokenType.Identifier, TokenType.Semicolon, TokenType.Error] },
    };

    public static TheoryData<string, int, int, string> ErrorDetailsData() => new()
    {
        { "@", 1, 1, "Unexpected character" },
        { "a @ b", 1, 3, "Unexpected character" },
        { "int x;\n  @", 2, 3, "Unexpected character" },
        { "a & b", 1, 3, "expected '&&'" },
        { "a | b", 1, 3, "expected '||'" },
        { "abcй", 1, 4, "Unexpected character" },
        { "x\n\nй", 3, 1, "Unexpected character" },
        { "x = \"abc", 1, 5, "Unterminated string" },
        { "\"a\\xb\"", 1, 1, "Unknown escape sequence" },
        { "\"a\nb\"", 1, 1, "Raw newline" },
        { "int x; /* oops", 1, 8, "Unterminated block comment" },
    };

    public static TheoryData<string, int, int> EofPositionData() => new()
    {
        { string.Empty, 1, 1 },
        { "   ", 1, 4 },
        { "int x;", 1, 7 },
        { "a\nb", 2, 2 },
        { "/* c */", 1, 8 },
    };

    public static TheoryData<string, int, int, int> TokenPositionData() => new()
    {
        { "int x;\n  y", 3, 2, 3 },
        { "/* a\nb */ x", 0, 2, 6 },
        { "a\r\nb", 1, 2, 1 },
    };

    [Theory]
    [MemberData(nameof(SingleTokenData))]
    public void NextToken_SingleToken_ReturnsTypeAndLexeme(string source, TokenType expectedType)
    {
        Token token = new Lexer(source).NextToken();
        Assert.Equal(expectedType, token.Type);
        Assert.Equal(source, token.Lexeme);
    }

    [Theory]
    [MemberData(nameof(IdentifiersData))]
    public void NextToken_Identifier_ReturnsIdentifierToken(string source)
    {
        Token token = new Lexer(source).NextToken();
        Assert.Equal(TokenType.Identifier, token.Type);
        Assert.Equal(source, token.Lexeme);
    }

    [Theory]
    [MemberData(nameof(TokenSequenceData))]
    public void TokenizeAll_ValidSource_ReturnsExpectedTypes(string source, TokenType[] expectedTypes)
    {
        List<Token> tokens = new Lexer(source).TokenizeAll();
        Assert.Equal(expectedTypes, tokens.Select(t => t.Type));
    }

    [Theory]
    [MemberData(nameof(InvalidCharactersData))]
    public void TokenizeAll_InvalidCharacter_StopsWithError(string source, TokenType[] expectedTypes)
    {
        List<Token> tokens = new Lexer(source).TokenizeAll();
        Assert.Equal(expectedTypes, tokens.Select(t => t.Type));
    }

    [Theory]
    [MemberData(nameof(InvalidLiteralsAndCommentsData))]
    public void TokenizeAll_InvalidLiteralOrComment_StopsWithError(string source, TokenType[] expectedTypes)
    {
        List<Token> tokens = new Lexer(source).TokenizeAll();
        Assert.Equal(expectedTypes, tokens.Select(t => t.Type));
    }

    [Theory]
    [MemberData(nameof(ErrorDetailsData))]
    public void TokenizeAll_InvalidSource_ReportsPositionAndMessage(string source, int line, int column, string messagePart)
    {
        Token error = new Lexer(source).TokenizeAll()[^1];
        Assert.Equal(line, error.Position.Line);
        Assert.Equal(column, error.Position.Column);
        Assert.Contains(messagePart, error.ErrorMessage);
    }

    [Theory]
    [MemberData(nameof(EofPositionData))]
    public void TokenizeAll_Eof_HasEmptyLexemeAndPosition(string source, int line, int column)
    {
        Token eof = new Lexer(source).TokenizeAll()[^1];
        Assert.Equal(string.Empty, eof.Lexeme);
        Assert.Equal(line, eof.Position.Line);
        Assert.Equal(column, eof.Position.Column);
    }

    [Theory]
    [MemberData(nameof(TokenPositionData))]
    public void TokenizeAll_MultiLineSource_ReportsTokenPosition(string source, int tokenIndex, int line, int column)
    {
        Token token = new Lexer(source).TokenizeAll()[tokenIndex];
        Assert.Equal(line, token.Position.Line);
        Assert.Equal(column, token.Position.Column);
    }

    [Fact]
    public void NextToken_AfterEof_KeepsReturningEof()
    {
        Lexer lexer = new Lexer("a");
        lexer.NextToken();
        lexer.NextToken();
        Token token = lexer.NextToken();
        Assert.Equal(TokenType.Eof, token.Type);
    }

    [Fact]
    public void NextToken_AfterError_ReturnsEof()
    {
        Lexer lexer = new Lexer("@ int");
        lexer.NextToken();
        Token token = lexer.NextToken();
        Assert.Equal(TokenType.Eof, token.Type);
    }
}