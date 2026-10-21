using Xunit;

namespace Nixie.Lexer.Tests;

public class LexerTests
{
    public static TheoryData<string, TokenType> KeywordsData() => new()
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
    };

    public static TheoryData<string, TokenType> OperatorsData() => new()
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
        { ".", TokenType.Dot },
    };

    public static TheoryData<string, TokenType> PunctuationData() => new()
    {
        { "[", TokenType.LBracket },
        { "]", TokenType.RBracket },
        { "(", TokenType.LParen },
        { ")", TokenType.RParen },
        { "{", TokenType.LBrace },
        { "}", TokenType.RBrace },
        { ",", TokenType.Comma },
        { ";", TokenType.Semicolon },
    };

    public static TheoryData<string> ValidIdentifiersData() => new()
    {
        "x",
        "_",
        "_var",
        "var_name",
        "test123",
        "_123",
        "ifVar",
        "int32",
        "str_print",
    };

    public static TheoryData<string> CaseSensitiveIdentifiersData() => new()
    {
        "IF",
        "Int",
        "STRING",
        "True",
        "NULL",
    };

    public static TheoryData<string> NumbersData() => new()
    {
        "0",
        "42",
        "100500",
        "007",
    };

    public static TheoryData<string> ValidStringsData() => new()
    {
        "\"\"",
        "\"hello\"",
        "\"a\\\"b\"",
        "\"a\\\\b\"",
        "\"a\\nb\"",
        "\"a\\tb\"",
        "\"a\\rb\"",
        "\"Line1\\nLine2\\t\\\"Quote\\\"\\\\ \\r\"",
    };

    public static TheoryData<string> WhitespaceData() => new()
    {
        " int",
        "\tint",
        "\rint",
        "\nint",
        "\fint",
        " \t\r\n\f  int",
    };

    public static TheoryData<string, TokenType[]> CommentsData() => new()
    {
        { "// comment", [TokenType.Eof] },
        { "// Однострочный комментарий\nint x;", [TokenType.Int, TokenType.Identifier, TokenType.Semicolon, TokenType.Eof] },
        { "/* Комментарий\nв несколько строк */ int /* ещё */ x;", [TokenType.Int, TokenType.Identifier, TokenType.Semicolon, TokenType.Eof] },
        { "/**/ int", [TokenType.Int, TokenType.Eof] },
        { "/* a */int", [TokenType.Int, TokenType.Eof] },
        { "a/*c*/b", [TokenType.Identifier, TokenType.Identifier, TokenType.Eof] },
        { "a // c", [TokenType.Identifier, TokenType.Eof] },

        // Вложенные комментарии не поддерживаются: первый "*/" закрывает комментарий, остаётся " */".
        { "/* /* */ */", [TokenType.Star, TokenType.Slash, TokenType.Eof] },
    };

    public static TheoryData<string, TokenType[]> TokenSequenceData() => new()
    {
        { string.Empty, [TokenType.Eof] },
        { "   ", [TokenType.Eof] },
        { "a==b", [TokenType.Identifier, TokenType.Equal, TokenType.Identifier, TokenType.Eof] },
        { "a!=b", [TokenType.Identifier, TokenType.NotEqual, TokenType.Identifier, TokenType.Eof] },
        { "a<=b", [TokenType.Identifier, TokenType.LessOrEqual, TokenType.Identifier, TokenType.Eof] },
        { "a>=b", [TokenType.Identifier, TokenType.GreaterOrEqual, TokenType.Identifier, TokenType.Eof] },
        { "a&&b", [TokenType.Identifier, TokenType.And, TokenType.Identifier, TokenType.Eof] },
        { "a||b", [TokenType.Identifier, TokenType.Or, TokenType.Identifier, TokenType.Eof] },
        { "a=!b", [TokenType.Identifier, TokenType.Assign, TokenType.Not, TokenType.Identifier, TokenType.Eof] },
        { "a===b", [TokenType.Identifier, TokenType.Equal, TokenType.Assign, TokenType.Identifier, TokenType.Eof] },
        { "a/b", [TokenType.Identifier, TokenType.Slash, TokenType.Identifier, TokenType.Eof] },
        { "-5", [TokenType.Minus, TokenType.Number, TokenType.Eof] },
        { "123abc", [TokenType.Number, TokenType.Identifier, TokenType.Eof] },
        { "arr[0].x", [TokenType.Identifier, TokenType.LBracket, TokenType.Number, TokenType.RBracket, TokenType.Dot, TokenType.Identifier, TokenType.Eof] },
    };

    public static TheoryData<string, TokenType[]> UnexpectedCharactersData() => new()
    {
        { "@", [TokenType.Error] },
        { "a @ b", [TokenType.Identifier, TokenType.Error] },
        { "&", [TokenType.Error] },
        { "a & b", [TokenType.Identifier, TokenType.Error] },
        { "|", [TokenType.Error] },
        { "a | b", [TokenType.Identifier, TokenType.Error] },
    };

    public static TheoryData<string, TokenType[]> NonAlphabetCharactersData() => new()
    {
        { "abcй", [TokenType.Identifier, TokenType.Error] },
        { "тест", [TokenType.Error] },
        { "x = й;", [TokenType.Identifier, TokenType.Assign, TokenType.Error] },
    };

    public static TheoryData<string, TokenType[]> UnterminatedStringsData() => new()
    {
        { "\"unterminated", [TokenType.Error] },
        { "\"unterminated \\\"", [TokenType.Error] },
        { "\"abc\\", [TokenType.Error] },
    };

    public static TheoryData<string, TokenType[]> InvalidStringContentData() => new()
    {
        { "\"hello \\x world\"", [TokenType.Error] },
        { "\"Line1\nLine2\"", [TokenType.Error] },
        { "\"Line1\rLine2\"", [TokenType.Error] },
    };

    public static TheoryData<string, TokenType[]> InvalidCommentsData() => new()
    {
        { "/*", [TokenType.Error] },
        { "/*/", [TokenType.Error] },
        { "/* Незакрытый комментарий", [TokenType.Error] },
        { "int x; /* oops", [TokenType.Int, TokenType.Identifier, TokenType.Semicolon, TokenType.Error] },
    };

    public static TheoryData<string, string> ErrorMessageData() => new()
    {
        { "@", "Unexpected character" },
        { "&", "expected '&&'" },
        { "|", "expected '||'" },
        { "\"unterminated", "Unterminated" },
        { "\"abc\\", "Unterminated" },
        { "\"hello \\x world\"", "Unknown escape sequence" },
    };

    public static TheoryData<string, int, int> ErrorPositionData() => new()
    {
        { "@", 1, 1 },
        { "a @ b", 1, 3 },
        { "int x;\n  @", 2, 3 },
        { "a & b", 1, 3 },
        { "a | b", 1, 3 },
        { "abcй", 1, 4 },
        { "x\n\nй", 3, 1 },
    };

    [Theory]
    [MemberData(nameof(KeywordsData))]
    public void NextToken_Keyword_ReturnsKeywordToken(string source, TokenType expectedType)
    {
        Token token = new Lexer(source).NextToken();
        Assert.Equal(expectedType, token.Type);
        Assert.Equal(source, token.Lexeme);
    }

    [Theory]
    [MemberData(nameof(OperatorsData))]
    public void NextToken_Operator_ReturnsOperatorToken(string source, TokenType expectedType)
    {
        Token token = new Lexer(source).NextToken();
        Assert.Equal(expectedType, token.Type);
        Assert.Equal(source, token.Lexeme);
    }

    [Theory]
    [MemberData(nameof(PunctuationData))]
    public void NextToken_Punctuation_ReturnsPunctuationToken(string source, TokenType expectedType)
    {
        Token token = new Lexer(source).NextToken();
        Assert.Equal(expectedType, token.Type);
        Assert.Equal(source, token.Lexeme);
    }

    [Theory]
    [MemberData(nameof(ValidIdentifiersData))]
    public void NextToken_ValidIdentifier_ReturnsIdentifierToken(string source)
    {
        Token token = new Lexer(source).NextToken();
        Assert.Equal(TokenType.Identifier, token.Type);
        Assert.Equal(source, token.Lexeme);
    }

    [Theory]
    [MemberData(nameof(CaseSensitiveIdentifiersData))]
    public void NextToken_KeywordInDifferentCase_ReturnsIdentifierToken(string source)
    {
        Token token = new Lexer(source).NextToken();
        Assert.Equal(TokenType.Identifier, token.Type);
        Assert.Equal(source, token.Lexeme);
    }

    [Theory]
    [MemberData(nameof(NumbersData))]
    public void NextToken_Number_ReturnsNumberToken(string source)
    {
        Token token = new Lexer(source).NextToken();
        Assert.Equal(TokenType.Number, token.Type);
        Assert.Equal(source, token.Lexeme);
    }

    [Theory]
    [MemberData(nameof(ValidStringsData))]
    public void NextToken_ValidString_ReturnsStringValueTokenWithQuotes(string source)
    {
        Token token = new Lexer(source).NextToken();
        Assert.Equal(TokenType.StringValue, token.Type);
        Assert.Equal(source, token.Lexeme);
    }

    [Theory]
    [MemberData(nameof(WhitespaceData))]
    public void TokenizeAll_Whitespace_IsSkipped(string source)
    {
        List<Token> tokens = new Lexer(source).TokenizeAll();
        Assert.Equal([TokenType.Int, TokenType.Eof], tokens.Select(t => t.Type));
    }

    [Theory]
    [MemberData(nameof(CommentsData))]
    public void TokenizeAll_Comments_AreSkipped(string source, TokenType[] expectedTypes)
    {
        List<Token> tokens = new Lexer(source).TokenizeAll();
        Assert.Equal(expectedTypes, tokens.Select(t => t.Type));
    }

    [Theory]
    [MemberData(nameof(TokenSequenceData))]
    public void TokenizeAll_Source_ReturnsExpectedTokenTypes(string source, TokenType[] expectedTypes)
    {
        List<Token> tokens = new Lexer(source).TokenizeAll();
        Assert.Equal(expectedTypes, tokens.Select(t => t.Type));
    }

    [Theory]
    [MemberData(nameof(UnexpectedCharactersData))]
    public void TokenizeAll_UnexpectedCharacter_StopsWithError(string source, TokenType[] expectedTypes)
    {
        List<Token> tokens = new Lexer(source).TokenizeAll();
        Assert.Equal(expectedTypes, tokens.Select(t => t.Type));
    }

    [Theory]
    [MemberData(nameof(NonAlphabetCharactersData))]
    public void TokenizeAll_NonAlphabetCharacter_StopsWithError(string source, TokenType[] expectedTypes)
    {
        List<Token> tokens = new Lexer(source).TokenizeAll();
        Assert.Equal(expectedTypes, tokens.Select(t => t.Type));
    }

    [Theory]
    [MemberData(nameof(UnterminatedStringsData))]
    public void TokenizeAll_UnterminatedString_StopsWithError(string source, TokenType[] expectedTypes)
    {
        List<Token> tokens = new Lexer(source).TokenizeAll();
        Assert.Equal(expectedTypes, tokens.Select(t => t.Type));
    }

    [Theory]
    [MemberData(nameof(InvalidStringContentData))]
    public void TokenizeAll_InvalidStringContent_StopsWithError(string source, TokenType[] expectedTypes)
    {
        List<Token> tokens = new Lexer(source).TokenizeAll();
        Assert.Equal(expectedTypes, tokens.Select(t => t.Type));
    }

    [Theory]
    [MemberData(nameof(InvalidCommentsData))]
    public void TokenizeAll_UnterminatedBlockComment_StopsWithError(string source, TokenType[] expectedTypes)
    {
        List<Token> tokens = new Lexer(source).TokenizeAll();
        Assert.Equal(expectedTypes, tokens.Select(t => t.Type));
    }

    [Theory]
    [MemberData(nameof(ErrorMessageData))]
    public void TokenizeAll_InvalidSource_ReportsErrorMessage(string source, string expectedMessagePart)
    {
        List<Token> tokens = new Lexer(source).TokenizeAll();
        Assert.Contains(expectedMessagePart, tokens[^1].ErrorMessage);
    }

    [Theory]
    [MemberData(nameof(ErrorPositionData))]
    public void TokenizeAll_InvalidSource_ReportsErrorPosition(string source, int expectedLine, int expectedColumn)
    {
        List<Token> tokens = new Lexer(source).TokenizeAll();
        Assert.Equal(expectedLine, tokens[^1].Position.Line);
        Assert.Equal(expectedColumn, tokens[^1].Position.Column);
    }

    [Fact]
    public void NextToken_AfterEof_KeepsReturningEof()
    {
        Lexer lexer = new Lexer(string.Empty);
        Token[] tokens = [lexer.NextToken(), lexer.NextToken(), lexer.NextToken()];
        Assert.Equal([TokenType.Eof, TokenType.Eof, TokenType.Eof], tokens.Select(t => t.Type));
    }

    [Fact]
    public void TokenizeAll_RealisticProgram_ReturnsExpectedTokenTypes()
    {
        List<Token> tokens = new Lexer("int main() { int x = 10; if (x >= 5) { str_print(\"ok\\n\"); } return 0; }").TokenizeAll();
        TokenType[] expectedTypes =
        [
            TokenType.Int, TokenType.Identifier, TokenType.LParen, TokenType.RParen, TokenType.LBrace,
            TokenType.Int, TokenType.Identifier, TokenType.Assign, TokenType.Number, TokenType.Semicolon,
            TokenType.If, TokenType.LParen, TokenType.Identifier, TokenType.GreaterOrEqual, TokenType.Number, TokenType.RParen, TokenType.LBrace,
            TokenType.Identifier, TokenType.LParen, TokenType.StringValue, TokenType.RParen, TokenType.Semicolon,
            TokenType.RBrace,
            TokenType.Return, TokenType.Number, TokenType.Semicolon,
            TokenType.RBrace,
            TokenType.Eof,
        ];
        Assert.Equal(expectedTypes, tokens.Select(t => t.Type));
    }
}