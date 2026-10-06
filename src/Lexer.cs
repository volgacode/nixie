using System.Collections.Generic;

namespace Nixie.Lexer;

public class Lexer
{
    private readonly string _source;
    private int _cursor = 0;
    private int _line = 1;
    private int _column = 1;

    public Lexer(string source)
    {
        _source = source ?? string.Empty;
    }

    private bool IsAtEnd => _cursor >= _source.Length;

    private char Peek() => IsAtEnd ? '\0' : _source[_cursor];

    private char PeekNext() => (_cursor + 1 >= _source.Length) ? '\0' : _source[_cursor + 1];

    private char Advance()
    {
        char current = Peek();
        _cursor++;
        if (current == '\n')
        {
            _line++;
            _column = 1;
        }
        else
        {
            _column++;
        }

        return current;
    }

    private bool Match(char expected)
    {
        if (IsAtEnd || _source[_cursor] != expected)
        {
            return false;
        }

        Advance();
        return true;
    }

    private void SkipWhitespaceAndComments()
    {
        while (!IsAtEnd)
        {
            char c = Peek();
            switch (c)
            {
                case ' ':
                case '\t':
                case '\r':
                case '\n':
                case '\f':
                    Advance();
                    break;

                case '/':
                    if (PeekNext() == '/')
                    {
                        Advance();
                        Advance();
                        while (Peek() != '\n' && !IsAtEnd)
                        {
                            Advance();
                        }
                    }
                    else if (PeekNext() == '*')
                    {
                        Advance();
                        Advance();
                        while (!IsAtEnd)
                        {
                            if (Peek() == '*' && PeekNext() == '/')
                            {
                                Advance();
                                Advance();
                                break;
                            }

                            Advance();
                        }
                    }
                    else
                    {
                        return;
                    }

                    break;

                default:
                    return;
            }
        }
    }

    public Token NextToken()
    {
        SkipWhitespaceAndComments();

        if (IsAtEnd)
        {
            return new Token(TokenType.Eof, string.Empty, new SourceLocation(_line, _column));
        }

        char c = Advance();

        if (char.IsLetter(c) || c == '_')
        {
            return ScanIdentifierOrKeyword();
        }

        if (char.IsDigit(c))
        {
            return ScanNumber();
        }

        if (c == '"')
        {
            return ScanString();
        }

        return c switch
        {
            '=' => Match('=') ? MakeToken(TokenType.Equal, 2) : MakeToken(TokenType.Assign, 1),
            '!' => Match('=') ? MakeToken(TokenType.NotEqual, 2) : MakeToken(TokenType.Not, 1),
            '<' => Match('=') ? MakeToken(TokenType.LessOrEqual, 2) : MakeToken(TokenType.Less, 1),
            '>' => Match('=') ? MakeToken(TokenType.GreaterOrEqual, 2) : MakeToken(TokenType.Greater, 1),
            '&' => Match('&') ? MakeToken(TokenType.And, 2) : MakeError("Unexpected character '&', expected '&&'"),
            '|' => Match('|') ? MakeToken(TokenType.Or, 2) : MakeError("Unexpected character '|', expected '||'"),
            '+' => MakeToken(TokenType.Plus, 1),
            '-' => MakeToken(TokenType.Minus, 1),
            '*' => MakeToken(TokenType.Star, 1),
            '/' => MakeToken(TokenType.Slash, 1),
            '.' => MakeToken(TokenType.Dot, 1),
            '[' => MakeToken(TokenType.LBracket, 1),
            ']' => MakeToken(TokenType.RBracket, 1),
            '(' => MakeToken(TokenType.LParen, 1),
            ')' => MakeToken(TokenType.RParen, 1),
            '{' => MakeToken(TokenType.LBrace, 1),
            '}' => MakeToken(TokenType.RBrace, 1),
            ',' => MakeToken(TokenType.Comma, 1),
            ';' => MakeToken(TokenType.Semicolon, 1),
            _ => MakeError($"Unexpected character: {c}"),
        };
    }

    private Token ScanIdentifierOrKeyword()
    {
        int start = _cursor - 1;
        while (char.IsLetterOrDigit(Peek()) || Peek() == '_')
        {
            Advance();
        }

        string text = _source[start.._cursor];
        TokenType kwType;
        TokenType type = Keywords.TryGetKeyword(text, out kwType) ? kwType : TokenType.Identifier;

        return new Token(type, text, new SourceLocation(_line, _column - text.Length));
    }

    private Token ScanNumber()
    {
        int start = _cursor - 1;
        while (char.IsDigit(Peek()))
        {
            Advance();
        }

        string text = _source[start.._cursor];
        return new Token(TokenType.Number, text, new SourceLocation(_line, _column - text.Length));
    }

    private Token ScanString()
    {
        int start = _cursor - 1;

        while (!IsAtEnd)
        {
            char c = Peek();

            if (c == '"')
            {
                Advance();
                string lexeme = _source[start.._cursor];
                return new Token(TokenType.StringValue, lexeme, new SourceLocation(_line, _column - lexeme.Length));
            }

            if (c == '\\')
            {
                Advance();
                if (IsAtEnd)
                {
                    break;
                }

                char escaped = Advance();
                if (escaped != '"' && escaped != '\\' && escaped != 'n' && escaped != 't' && escaped != 'r')
                {
                    return MakeError($"Unknown escape sequence: \\{escaped}");
                }

                continue;
            }

            Advance();
        }

        return MakeError("Unterminated string literal");
    }

    private Token MakeToken(TokenType type, int length)
    {
        string lexeme = _source[(_cursor - length).._cursor];
        return new Token(type, lexeme, new SourceLocation(_line, _column - length));
    }

    private Token MakeError(string message)
    {
        return new Token(TokenType.Error, string.Empty, new SourceLocation(_line, _column), message);
    }

    public List<Token> TokenizeAll()
    {
        List<Token> tokens = new List<Token>();
        while (true)
        {
            Token token = NextToken();
            tokens.Add(token);
            if (token.Type is TokenType.Eof or TokenType.Error)
            {
                break;
            }
        }

        return tokens;
    }
}