namespace Lexing;

public class Lexer(string source)
{
    private int _index;
    private SourcePosition _position = new(1, 1);
    private SourcePosition _tokenStart;
    private int _tokenStartIndex;
    private bool _faulted;

    private bool IsAtEnd => _index >= source.Length;

    public Token NextToken()
    {
        if (_faulted)
        {
            BeginToken();
            return MakeToken(TokenType.Eof);
        }

        SkipTrivia();
        BeginToken();

        if (IsAtEnd)
        {
            return MakeToken(TokenType.Eof);
        }

        char c = Advance();

        if (IsAsciiLetter(c) || c == '_')
        {
            return ScanIdentifierOrKeyword();
        }

        if (IsAsciiDigit(c))
        {
            return ScanNumber();
        }

        return c switch
        {
            '=' => Match('=') ? MakeToken(TokenType.Equal) : MakeToken(TokenType.Assign),
            '!' => Match('=') ? MakeToken(TokenType.NotEqual) : MakeToken(TokenType.Not),
            '<' => Match('=') ? MakeToken(TokenType.LessOrEqual) : MakeToken(TokenType.Less),
            '>' => Match('=') ? MakeToken(TokenType.GreaterOrEqual) : MakeToken(TokenType.Greater),
            '&' => Match('&') ? MakeToken(TokenType.And) : MakeError("Unexpected character '&', expected '&&'"),
            '|' => Match('|') ? MakeToken(TokenType.Or) : MakeError("Unexpected character '|', expected '||'"),
            '+' => MakeToken(TokenType.Plus),
            '-' => MakeToken(TokenType.Minus),
            '*' => MakeToken(TokenType.Star),
            '/' => MakeToken(TokenType.Slash),
            '(' => MakeToken(TokenType.LParen),
            ')' => MakeToken(TokenType.RParen),
            '{' => MakeToken(TokenType.LBrace),
            '}' => MakeToken(TokenType.RBrace),
            '[' => MakeToken(TokenType.LBracket),
            ']' => MakeToken(TokenType.RBracket),
            '.' => MakeToken(TokenType.Dot),
            ',' => MakeToken(TokenType.Comma),
            ';' => MakeToken(TokenType.Semicolon),
            '"' => ScanString(),
            _ => MakeUnexpectedCharacterError(c),
        };
    }

    public List<Token> TokenizeAll()
    {
        List<Token> tokens = new();

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

    private void BeginToken()
    {
        _tokenStart = _position;
        _tokenStartIndex = _index;
    }

    private char Peek(int offset = 0)
    {
        int index = _index + offset;

        if (index >= source.Length)
        {
            return '\0';
        }

        return source[index];
    }

    private char Advance()
    {
        char current = Peek();
        _index++;

        if (current == '\n' || (current == '\r' && Peek() != '\n'))
        {
            _position = new SourcePosition(_position.Line + 1, 1);
        }
        else
        {
            _position = new SourcePosition(_position.Line, _position.Column + 1);
        }

        return current;
    }

    private bool Match(char expected)
    {
        if (IsAtEnd || source[_index] != expected)
        {
            return false;
        }

        Advance();
        return true;
    }

    private void SkipTrivia()
    {
        while (!IsAtEnd)
        {
            if (IsWhitespace(Peek()))
            {
                Advance();
            }
            else if (Peek() == '/' && Peek(1) == '/')
            {
                SkipLineComment();
            }
            else
            {
                break;
            }
        }
    }

    private void SkipLineComment()
    {
        while (!IsAtEnd && Peek() is not ('\r' or '\n'))
        {
            Advance();
        }
    }

    private static bool IsWhitespace(char c)
    {
        return c is ' ' or '\t' or '\r' or '\n' or '\f';
    }

    private Token ScanIdentifierOrKeyword()
    {
        while (IsAsciiLetterOrDigit(Peek()) || Peek() == '_')
        {
            Advance();
        }

        string lexeme = CurrentLexeme();

        if (Keywords.TryGetKeyword(lexeme, out TokenType keywordType))
        {
            return new Token(keywordType, lexeme, _tokenStart);
        }

        return new Token(TokenType.Identifier, lexeme, _tokenStart);
    }

    private Token ScanNumber()
    {
        while (IsAsciiDigit(Peek()))
        {
            Advance();
        }

        return MakeToken(TokenType.Number);
    }

    private Token ScanString()
    {
        while (!IsAtEnd)
        {
            char c = Advance();
            if (c == '"')
            {
                return MakeToken(TokenType.StringLiteral);
            }

            if (c is '\r' or '\n')
            {
                return MakeError("Newline in string literal");
            }

            if (c == '\\')
            {
                if (IsAtEnd)
                {
                    return MakeError("Unterminated escape sequence in string literal");
                }

                if (Advance() is not ('"' or '\\' or 'n' or 'r' or 't'))
                {
                    return MakeError("Invalid escape sequence in string literal");
                }
            }
        }

        return MakeError("Unterminated string literal");
    }

    private string CurrentLexeme()
    {
        return source.Substring(_tokenStartIndex, _index - _tokenStartIndex);
    }

    private Token MakeToken(TokenType type)
    {
        return new Token(type, CurrentLexeme(), _tokenStart);
    }

    private Token MakeUnexpectedCharacterError(char first)
    {
        if (char.IsHighSurrogate(first) && char.IsLowSurrogate(Peek()))
        {
            Advance();
        }

        return MakeError($"Unexpected character '{CurrentLexeme()}'");
    }

    private Token MakeError(string message)
    {
        _faulted = true;
        return new Token(TokenType.Error, CurrentLexeme(), _tokenStart, message);
    }

    private static bool IsAsciiLetter(char c)
    {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
    }

    private static bool IsAsciiDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

    private static bool IsAsciiLetterOrDigit(char c)
    {
        return IsAsciiLetter(c) || IsAsciiDigit(c);
    }
}