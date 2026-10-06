using System.Collections.Generic;

using FluentAssertions;

using Nixie.Lexer;

using Xunit;

namespace Nixie.Lexer.Tests;

public class LexerTests
{
    [Theory]
    [InlineData("bool", TokenType.Bool)]
    [InlineData("int", TokenType.Int)]
    [InlineData("string", TokenType.String)]
    [InlineData("array", TokenType.Array)]
    [InlineData("if", TokenType.If)]
    [InlineData("else", TokenType.Else)]
    [InlineData("while", TokenType.While)]
    [InlineData("for", TokenType.For)]
    [InlineData("void", TokenType.Void)]
    [InlineData("return", TokenType.Return)]
    [InlineData("struct", TokenType.Struct)]
    [InlineData("true", TokenType.True)]
    [InlineData("false", TokenType.False)]
    [InlineData("null", TokenType.Null)]
    public void NextToken_ShouldRecognizeAllKeywords(string source, TokenType expectedType)
    {
        Lexer lexer = new Lexer(source);
        Token token = lexer.NextToken();

        token.Type.Should().Be(expectedType);
        token.Lexeme.Should().Be(source);
    }

    [Theory]
    [InlineData("=", TokenType.Assign)]
    [InlineData("==", TokenType.Equal)]
    [InlineData("!=", TokenType.NotEqual)]
    [InlineData("<", TokenType.Less)]
    [InlineData("<=", TokenType.LessOrEqual)]
    [InlineData(">", TokenType.Greater)]
    [InlineData(">=", TokenType.GreaterOrEqual)]
    [InlineData("&&", TokenType.And)]
    [InlineData("||", TokenType.Or)]
    [InlineData("!", TokenType.Not)]
    [InlineData("+", TokenType.Plus)]
    [InlineData("-", TokenType.Minus)]
    [InlineData("*", TokenType.Star)]
    [InlineData("/", TokenType.Slash)]
    [InlineData(".", TokenType.Dot)]
    public void NextToken_ShouldRecognizeAllOperators(string source, TokenType expectedType)
    {
        Lexer lexer = new Lexer(source);
        Token token = lexer.NextToken();

        token.Type.Should().Be(expectedType);
        token.Lexeme.Should().Be(source);
    }

    [Theory]
    [InlineData("[", TokenType.LBracket)]
    [InlineData("]", TokenType.RBracket)]
    [InlineData("(", TokenType.LParen)]
    [InlineData(")", TokenType.RParen)]
    [InlineData("{", TokenType.LBrace)]
    [InlineData("}", TokenType.RBrace)]
    [InlineData(",", TokenType.Comma)]
    [InlineData(";", TokenType.Semicolon)]
    public void NextToken_ShouldRecognizeAllPunctuation(string source, TokenType expectedType)
    {
        Lexer lexer = new Lexer(source);
        Token token = lexer.NextToken();

        token.Type.Should().Be(expectedType);
        token.Lexeme.Should().Be(source);
    }

    [Theory]
    [InlineData("x")]
    [InlineData("_var")]
    [InlineData("var_name")]
    [InlineData("test123")]
    [InlineData("_123")]
    [InlineData("ifVar")]
    [InlineData("int32")]
    public void NextToken_ShouldRecognizeIdentifiersWithNuances(string source)
    {
        Lexer lexer = new Lexer(source);
        Token token = lexer.NextToken();

        token.Type.Should().Be(TokenType.Identifier);
        token.Lexeme.Should().Be(source);
    }

    [Fact]
    public void NextToken_ShouldBeCaseSensitive()
    {
        Lexer lexer = new Lexer("IF Int STRING");

        lexer.NextToken().Type.Should().Be(TokenType.Identifier);
        lexer.NextToken().Type.Should().Be(TokenType.Identifier);
        lexer.NextToken().Type.Should().Be(TokenType.Identifier);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("42")]
    [InlineData("100500")]
    [InlineData("000123")]
    public void NextToken_ShouldRecognizeNumbers(string source)
    {
        Lexer lexer = new Lexer(source);
        Token token = lexer.NextToken();

        token.Type.Should().Be(TokenType.Number);
        token.Lexeme.Should().Be(source);
    }

    [Fact]
    public void NextToken_ShouldRecognizeEmptyString()
    {
        Lexer lexer = new Lexer("\"\"");
        Token token = lexer.NextToken();

        token.Type.Should().Be(TokenType.StringValue);
        token.Lexeme.Should().Be("\"\"");
    }

    [Fact]
    public void NextToken_ShouldHandleEscapeSequencesInString()
    {
        string source = "\"Line1\\nLine2\\t\\\"Quote\\\"\\\\ \\r\"";
        Lexer lexer = new Lexer(source);
        Token token = lexer.NextToken();

        token.Type.Should().Be(TokenType.StringValue);
        token.Lexeme.Should().Be(source);
    }

    [Fact]
    public void NextToken_ShouldReturnError_OnUnknownEscapeSequence()
    {
        Lexer lexer = new Lexer("\"hello \\x world\"");
        Token token = lexer.NextToken();

        token.Type.Should().Be(TokenType.Error);
        token.ErrorMessage.Should().Contain("Unknown escape sequence");
    }

    [Theory]
    [InlineData("\"unterminated")]
    [InlineData("\"unterminated \\\"")]
    public void NextToken_ShouldReturnError_OnUnterminatedStringAtEof(string source)
    {
        Lexer lexer = new Lexer(source);
        Token token = lexer.NextToken();

        token.Type.Should().Be(TokenType.Error);
        token.ErrorMessage.Should().Contain("Unterminated string");
    }

    [Fact]
    public void NextToken_ShouldSkipAllTypesOfWhitespace()
    {
        string source = " \t\r\n\f  int  \t\n  ";
        Lexer lexer = new Lexer(source);

        Token token = lexer.NextToken();
        token.Type.Should().Be(TokenType.Int);

        Token eof = lexer.NextToken();
        eof.Type.Should().Be(TokenType.Eof);
    }

    [Fact]
    public void NextToken_ShouldSkipSingleLineComments()
    {
        string source = "// Однострочный комментарий\nint x; // Другой комментарий";
        Lexer lexer = new Lexer(source);
        List<Token> tokens = lexer.TokenizeAll();

        tokens.Should().SatisfyRespectively(
            t => t.Type.Should().Be(TokenType.Int),
            t => t.Type.Should().Be(TokenType.Identifier),
            t => t.Type.Should().Be(TokenType.Semicolon),
            t => t.Type.Should().Be(TokenType.Eof));
    }

    [Fact]
    public void NextToken_ShouldSkipMultiLineComments()
    {
        string source = "/* Старт комментария\n в несколько строк */ int /* еще */ x;";
        Lexer lexer = new Lexer(source);
        List<Token> tokens = lexer.TokenizeAll();

        tokens.Should().SatisfyRespectively(
            t => t.Type.Should().Be(TokenType.Int),
            t => t.Type.Should().Be(TokenType.Identifier),
            t => t.Type.Should().Be(TokenType.Semicolon),
            t => t.Type.Should().Be(TokenType.Eof));
    }

    [Fact]
    public void NextToken_ShouldReturnError_OnSingleAmpersand()
    {
        Lexer lexer = new Lexer("&");
        Token token = lexer.NextToken();

        token.Type.Should().Be(TokenType.Error);
        token.ErrorMessage.Should().Contain("expected '&&'");
    }

    [Fact]
    public void NextToken_ShouldReturnError_OnSinglePipe()
    {
        Lexer lexer = new Lexer("|");
        Token token = lexer.NextToken();

        token.Type.Should().Be(TokenType.Error);
        token.ErrorMessage.Should().Contain("expected '||'");
    }

    [Fact]
    public void NextToken_ShouldReturnError_OnUnexpectedCharacter()
    {
        Lexer lexer = new Lexer("@");
        Token token = lexer.NextToken();

        token.Type.Should().Be(TokenType.Error);
        token.ErrorMessage.Should().Contain("Unexpected character");
    }

    [Fact]
    public void NextToken_ShouldHandleNullSourceGracefully()
    {
        Lexer lexer = new Lexer(null!);
        Token token = lexer.NextToken();

        token.Type.Should().Be(TokenType.Eof);
    }

    [Fact]
    public void NextToken_ShouldTrackSourceLocationWithLinesAndColumns()
    {
        string source = "int x;\n  y = 5;";
        Lexer lexer = new Lexer(source);
        List<Token> tokens = lexer.TokenizeAll();

        tokens[0].Location.Should().Be(new SourceLocation(1, 1));
        tokens[1].Location.Should().Be(new SourceLocation(1, 5));
        tokens[2].Location.Should().Be(new SourceLocation(1, 6));
        tokens[3].Location.Should().Be(new SourceLocation(2, 3));
        tokens[4].Location.Should().Be(new SourceLocation(2, 5));
        tokens[5].Location.Should().Be(new SourceLocation(2, 7));
    }

    [Fact]
    public void TokenTypeExtensions_ToRepresentation_ShouldReturnCorrectStrings()
    {
        TokenType.If.ToRepresentation().Should().Be("if");
        TokenType.Equal.ToRepresentation().Should().Be("==");
        TokenType.LBrace.ToRepresentation().Should().Be("{");
        TokenType.Identifier.ToRepresentation().Should().Be(string.Empty);
    }

    [Fact]
    public void TokenAndLocation_ToString_ShouldFormatCorrectly()
    {
        SourceLocation loc = new SourceLocation(2, 5);
        loc.ToString().Should().Be("(2:5)");

        Token token = new Token(TokenType.Int, "int", loc);
        token.ToString().Should().Be("Int ('int') at (2:5)");

        Token errToken = new Token(TokenType.Error, string.Empty, loc, "Test error");
        errToken.ToString().Should().Be("Error at (2:5): Test error");
    }

    [Fact]
    public void LexerException_ShouldConstructCorrectly()
    {
        SourceLocation loc = new SourceLocation(3, 10);
        LexerException ex = new LexerException("Invalid token", loc);

        ex.Location.Should().Be(loc);
        ex.Message.Should().Be("Invalid token at (3:10)");
    }
}