using Lexing;

using Xunit;

namespace LexingTests;

public class LexingTests
{
    public static TheoryData<string, TokenType> KeywordData() => new()
    {
        { "bool", TokenType.Bool },
        { "int", TokenType.Int },
        { "string", TokenType.String },
        { "array", TokenType.Array },
        { "struct", TokenType.Struct },
        { "if", TokenType.If },
        { "else", TokenType.Else },
        { "while", TokenType.While },
        { "for", TokenType.For },
        { "void", TokenType.Void },
        { "return", TokenType.Return },
        { "true", TokenType.True },
        { "false", TokenType.False },
    };

    public static TheoryData<string, TokenType> OperatorData() => new()
    {
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
    };

    public static TheoryData<string, TokenType> DelimiterData() => new()
    {
        { "(", TokenType.LParen },
        { ")", TokenType.RParen },
        { "{", TokenType.LBrace },
        { "}", TokenType.RBrace },
        { "[", TokenType.LBracket },
        { "]", TokenType.RBracket },
        { ".", TokenType.Dot },
        { ",", TokenType.Comma },
        { ";", TokenType.Semicolon },
    };

    public static TheoryData<string, TokenType> NumberData() => new()
    {
        { "0", TokenType.Number },
        { "7", TokenType.Number },
        { "007", TokenType.Number },
        { "42", TokenType.Number },
        { "2147483647", TokenType.Number },
        { "2147483648", TokenType.Number },
        { "99999999999", TokenType.Number },
    };

    public static TheoryData<string> StringLiteralData() => new()
    {
        "\"\"",
        "\"Hello\"",
        "\"Hello World\"",
        "\"Привет\"",
        "\"Привет, мир!\"",
        "\"Текст с пробелами и кириллицей\"",
        "\"  несколько пробелов  \"",
        "\"Ёжик съел яблоко\"",
        "\"Unicode: 😀\"",
        "\"quote: \\\"inside\\\"\"",
        "\"path: C:\\\\folder\\\\file\"",
        "\"line1\\nline2\"",
        "\"line1\\rline2\"",
        "\"column1\\tcolumn2\"",
        "\"// not a comment\"",
        "\"array\"",
        "\"struct\"",
        "\"[0]\"",
        "\"a.b\"",
        "\"arr[0].x\"",
    };

    public static TheoryData<string> IdentifierData() => new()
    {
        "x",
        "_",
        "__",
        "_var",
        "a1b2",
        "test123",
        "print",
        "integer",
        "returns",
        "trueValue",
        "ifVar",
        "IF",
        "Int",
        "read",
        "abs",
        "min",
        "max",
        "main",
        "String",
        "STRING",
        "stringValue",
        "string1",
        "Array",
        "ARRAY",
        "array1",
        "arrayValue",
        "Struct",
        "STRUCT",
        "struct1",
        "structValue",
    };

    public static TheoryData<string> WhitespaceData() => new()
    {
        " int",
        "\tint",
        "\nint",
        "\rint",
        "\fint",
        "\r\nint",
        " \t\r\n\f int",
    };

    public static TheoryData<string> CommentData() => new()
    {
        "// comment\nint",
        "// comment\r\nint",
        "// comment\rint",
        "//\nint",
        "// first\n \t// second\r\n\nint// last",
        " \t// @ # $ \" комментарий /* */\nint",
        "int// comment",
        "int // comment\n\t",
    };

    public static TheoryData<string, TokenType[]> TokenSequenceData() => new()
    {
        { string.Empty, [TokenType.Eof] },
        { "a==b", [TokenType.Identifier, TokenType.Equal, TokenType.Identifier, TokenType.Eof] },
        { "a!=b", [TokenType.Identifier, TokenType.NotEqual, TokenType.Identifier, TokenType.Eof] },
        { "a<b", [TokenType.Identifier, TokenType.Less, TokenType.Identifier, TokenType.Eof] },
        { "a<=b", [TokenType.Identifier, TokenType.LessOrEqual, TokenType.Identifier, TokenType.Eof] },
        { "a>b", [TokenType.Identifier, TokenType.Greater, TokenType.Identifier, TokenType.Eof] },
        { "a>=b", [TokenType.Identifier, TokenType.GreaterOrEqual, TokenType.Identifier, TokenType.Eof] },
        { "a&&b", [TokenType.Identifier, TokenType.And, TokenType.Identifier, TokenType.Eof] },
        { "a||b", [TokenType.Identifier, TokenType.Or, TokenType.Identifier, TokenType.Eof] },
        { "a&&!b||c", [TokenType.Identifier, TokenType.And, TokenType.Not, TokenType.Identifier, TokenType.Or, TokenType.Identifier, TokenType.Eof] },
        { "a=!b", [TokenType.Identifier, TokenType.Assign, TokenType.Not, TokenType.Identifier, TokenType.Eof] },
        { "a!==b", [TokenType.Identifier, TokenType.NotEqual, TokenType.Assign, TokenType.Identifier, TokenType.Eof] },
        { "a/b", [TokenType.Identifier, TokenType.Slash, TokenType.Identifier, TokenType.Eof] },
        { "//", [TokenType.Eof] },
        { "// comment", [TokenType.Eof] },
        { " \t// first\n// second\r\n// last", [TokenType.Eof] },
        { "a// comment\nb", [TokenType.Identifier, TokenType.Identifier, TokenType.Eof] },
        { "1// comment\n2", [TokenType.Number, TokenType.Number, TokenType.Eof] },
        { "8/2// comment\n+1", [TokenType.Number, TokenType.Slash, TokenType.Number, TokenType.Plus, TokenType.Number, TokenType.Eof] },
        { "a / / b", [TokenType.Identifier, TokenType.Slash, TokenType.Slash, TokenType.Identifier, TokenType.Eof] },
        { "/*", [TokenType.Slash, TokenType.Star, TokenType.Eof] },
        { "1+2", [TokenType.Number, TokenType.Plus, TokenType.Number, TokenType.Eof] },
        { "x=-1", [TokenType.Identifier, TokenType.Assign, TokenType.Minus, TokenType.Number, TokenType.Eof] },
        { "if(x)", [TokenType.If, TokenType.LParen, TokenType.Identifier, TokenType.RParen, TokenType.Eof] },
        { "else{", [TokenType.Else, TokenType.LBrace, TokenType.Eof] },
        { "int x;", [TokenType.Int, TokenType.Identifier, TokenType.Semicolon, TokenType.Eof] },
        { "int x = 1;", [TokenType.Int, TokenType.Identifier, TokenType.Assign, TokenType.Number, TokenType.Semicolon, TokenType.Eof] },
        { "int x = 2147483648;", [TokenType.Int, TokenType.Identifier, TokenType.Assign, TokenType.Number, TokenType.Semicolon, TokenType.Eof] },
        { "int x = -2147483648;", [TokenType.Int, TokenType.Identifier, TokenType.Assign, TokenType.Minus, TokenType.Number, TokenType.Semicolon, TokenType.Eof] },
        { "int x = 99999999999;", [TokenType.Int, TokenType.Identifier, TokenType.Assign, TokenType.Number, TokenType.Semicolon, TokenType.Eof] },
        { "bool b = true;", [TokenType.Bool, TokenType.Identifier, TokenType.Assign, TokenType.True, TokenType.Semicolon, TokenType.Eof] },
        { "while (i < 10) { i = i + 1; }", [TokenType.While, TokenType.LParen, TokenType.Identifier, TokenType.Less, TokenType.Number, TokenType.RParen, TokenType.LBrace, TokenType.Identifier, TokenType.Assign, TokenType.Identifier, TokenType.Plus, TokenType.Number, TokenType.Semicolon, TokenType.RBrace, TokenType.Eof] },
        { "int main() { return 0; }", [TokenType.Int, TokenType.Identifier, TokenType.LParen, TokenType.RParen, TokenType.LBrace, TokenType.Return, TokenType.Number, TokenType.Semicolon, TokenType.RBrace, TokenType.Eof] },
        { "min(1, 2)", [TokenType.Identifier, TokenType.LParen, TokenType.Number, TokenType.Comma, TokenType.Number, TokenType.RParen, TokenType.Eof] },
        { "int main() { print(min(1, 2)); }", [TokenType.Int, TokenType.Identifier, TokenType.LParen, TokenType.RParen, TokenType.LBrace, TokenType.Identifier, TokenType.LParen, TokenType.Identifier, TokenType.LParen, TokenType.Number, TokenType.Comma, TokenType.Number, TokenType.RParen, TokenType.RParen, TokenType.Semicolon, TokenType.RBrace, TokenType.Eof] },
        { "string value;", [TokenType.String, TokenType.Identifier, TokenType.Semicolon, TokenType.Eof] },
        { "string \"hello\"", [TokenType.String, TokenType.StringLiteral, TokenType.Eof] },
        { "arr[0]", [TokenType.Identifier, TokenType.LBracket, TokenType.Number, TokenType.RBracket, TokenType.Eof] },
        { "arr[i]", [TokenType.Identifier, TokenType.LBracket, TokenType.Identifier, TokenType.RBracket, TokenType.Eof] },
        { "arr[1][2]", [TokenType.Identifier, TokenType.LBracket, TokenType.Number, TokenType.RBracket, TokenType.LBracket, TokenType.Number, TokenType.RBracket, TokenType.Eof] },
        { "a.b", [TokenType.Identifier, TokenType.Dot, TokenType.Identifier, TokenType.Eof] },
        { "a.b.c", [TokenType.Identifier, TokenType.Dot, TokenType.Identifier, TokenType.Dot, TokenType.Identifier, TokenType.Eof] },
        { "arr[0].x", [TokenType.Identifier, TokenType.LBracket, TokenType.Number, TokenType.RBracket, TokenType.Dot, TokenType.Identifier, TokenType.Eof] },
        { "array int", [TokenType.Array, TokenType.Int, TokenType.Eof] },
        { "struct Point", [TokenType.Struct, TokenType.Identifier, TokenType.Eof] },
        { "struct Point { int x; }", [TokenType.Struct, TokenType.Identifier, TokenType.LBrace, TokenType.Int, TokenType.Identifier, TokenType.Semicolon, TokenType.RBrace, TokenType.Eof] },
    };

    public static TheoryData<string, TokenType[]> InvalidCharacterData() => new()
    {
        { "@", [TokenType.Error] },
        { "#", [TokenType.Error] },
        { "$", [TokenType.Error] },
        { "%", [TokenType.Error] },
        { "^", [TokenType.Error] },
        { "~", [TokenType.Error] },
        { "?", [TokenType.Error] },
        { ":", [TokenType.Error] },
        { "'", [TokenType.Error] },
        { "`", [TokenType.Error] },
        { "\\", [TokenType.Error] },
        { "&", [TokenType.Error] },
        { "|", [TokenType.Error] },
        { "\v", [TokenType.Error] },
        { "\u00A0", [TokenType.Error] },
        { "a @ b", [TokenType.Identifier, TokenType.Error] },
        { "a | b", [TokenType.Identifier, TokenType.Error] },
        { "int x = 5 @;", [TokenType.Int, TokenType.Identifier, TokenType.Assign, TokenType.Number, TokenType.Error] },
        { "тест", [TokenType.Error] },
        { "abcй", [TokenType.Identifier, TokenType.Error] },
        { "\u0661\u0662\u0663", [TokenType.Error] },
        { "1\u0662", [TokenType.Number, TokenType.Error] },
        { "\"", [TokenType.Error] },
    };

    public static TheoryData<string, int, int, string> ErrorDetailsData() => new()
    {
        { "@", 1, 1, "Unexpected character" },
        { "a @ b", 1, 3, "Unexpected character" },
        { "int x;\n  @", 2, 3, "Unexpected character" },
        { "int x = 5 @;", 1, 11, "Unexpected character" },
        { "abcй", 1, 4, "Unexpected character" },
        { "x\n\nй", 3, 1, "Unexpected character" },
        { "// @\n  @", 2, 3, "Unexpected character" },
        { "// @\r\n  @", 2, 3, "Unexpected character" },
        { "// @\r  @", 2, 3, "Unexpected character" },
        { "a & b", 1, 3, "expected '&&'" },
        { "a | b", 1, 3, "expected '||'" },
        { "\"hello\\q\"", 1, 1, "Invalid escape sequence in string literal" },
        { "\"hello\\", 1, 1, "Unterminated escape sequence in string literal" },
        { "\"line1\nline2\"", 1, 1, "Newline in string literal" },
        { "\"line1\rline2\"", 1, 1, "Newline in string literal" },
        { "\"hello", 1, 1, "Unterminated string literal" },
    };

    public static TheoryData<string, int, int> EofPositionData() => new()
    {
        { string.Empty, 1, 1 },
        { "   ", 1, 4 },
        { "\n", 2, 1 },
        { "int x;", 1, 7 },
        { "a\nb", 2, 2 },
        { "a\r\nb\r\n", 3, 1 },
        { "//", 1, 3 },
        { "// comment", 1, 11 },
        { "// comment\n", 2, 1 },
        { "// comment\r\n", 2, 1 },
        { "// comment\r", 2, 1 },
        { "a//", 1, 4 },
    };

    public static TheoryData<string, int, int, int> TokenPositionData() => new()
    {
        { "int x = 1;", 3, 1, 9 },
        { "  \t x", 0, 1, 5 },
        { "int x;\n  y", 3, 2, 3 },
        { "a\r\nb", 1, 2, 1 },
        { "a\rb", 1, 2, 1 },
        { "// comment\n  int", 0, 2, 3 },
        { "// comment\r\n  int", 0, 2, 3 },
        { "// comment\r  int", 0, 2, 3 },
        { "a// first\n// second\r\n  b", 1, 3, 3 },
        { "arr[0]", 1, 1, 4 },
        { "arr[0]", 2, 1, 5 },
        { "arr[0]", 3, 1, 6 },
        { "a.b", 1, 1, 2 },
        { "a.b", 2, 1, 3 },
        { "struct Point", 1, 1, 8 },
    };

    public static TheoryData<string> ExhaustedSourceData() => new()
    {
        "a",
        "@ int",
    };

    [Theory]
    [MemberData(nameof(KeywordData))]
    [MemberData(nameof(OperatorData))]
    [MemberData(nameof(DelimiterData))]
    [MemberData(nameof(NumberData))]
    public void NextToken_SingleToken_ReturnsTypeAndLexeme(string source, TokenType expectedType)
    {
        Token token = new Lexer(source).NextToken();

        Assert.Equal(expectedType, token.Type);
        Assert.Equal(source, token.Lexeme);
    }

    [Theory]
    [MemberData(nameof(IdentifierData))]
    public void NextToken_Identifier_ReturnsIdentifierToken(string source)
    {
        Token token = new Lexer(source).NextToken();

        Assert.Equal(TokenType.Identifier, token.Type);
        Assert.Equal(source, token.Lexeme);
    }

    [Theory]
    [MemberData(nameof(StringLiteralData))]
    public void NextToken_StringLiteral_ReturnsStringLiteralToken(string source)
    {
        Token token = new Lexer(source).NextToken();

        Assert.Equal(TokenType.StringLiteral, token.Type);
        Assert.Equal(source, token.Lexeme);
    }

    [Theory]
    [MemberData(nameof(WhitespaceData))]
    [MemberData(nameof(CommentData))]
    public void TokenizeAll_Trivia_IsSkipped(string source)
    {
        TokenType[] expectedTypes =
            [TokenType.Int, TokenType.Eof];

        List<Token> tokens = new Lexer(source).TokenizeAll();

        Assert.Equal(expectedTypes, tokens.Select(t => t.Type));
    }

    [Theory]
    [MemberData(nameof(TokenSequenceData))]
    public void TokenizeAll_ValidSource_ReturnsExpectedTypes(string source, TokenType[] expectedTypes)
    {
        List<Token> tokens = new Lexer(source).TokenizeAll();

        Assert.Equal(expectedTypes, tokens.Select(t => t.Type));
    }

    [Theory]
    [MemberData(nameof(InvalidCharacterData))]
    public void TokenizeAll_InvalidCharacter_StopsWithError(string source, TokenType[] expectedTypes)
    {
        List<Token> tokens = new Lexer(source).TokenizeAll();

        Assert.Equal(expectedTypes, tokens.Select(t => t.Type));
    }

    [Theory]
    [MemberData(nameof(ErrorDetailsData))]
    public void TokenizeAll_InvalidSource_ReportsPositionAndMessage(string source, int line, int column, string messagePart)
    {
        Token error = new Lexer(source).TokenizeAll()[^1];

        Assert.Equal(TokenType.Error, error.Type);
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

    [Theory]
    [MemberData(nameof(ExhaustedSourceData))]
    public void NextToken_AfterEofOrError_ReturnsEof(string source)
    {
        Lexer lexer = new Lexer(source);
        lexer.TokenizeAll();

        Token token = lexer.NextToken();

        Assert.Equal(TokenType.Eof, token.Type);
    }
}