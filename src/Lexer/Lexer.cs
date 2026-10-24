namespace Nixie.Lexer;

public class Lexer
{
    private readonly string _source;
    private int _index;
    private SourcePosition _position;
    private SourcePosition _tokenStart;
    private int _tokenStartIndex;
    private bool _faulted;

    public Lexer(string source)
    {
        _source = source;
        _position = new SourcePosition(1, 1);
        _tokenStart = _position;
    }

    private bool IsAtEnd => _index >= _source.Length;

    public Token NextToken()
    {
        if (_faulted)
        {
            BeginToken();
            return MakeToken(TokenType.Eof);
        }

        Token? skipError = SkipWhitespaceAndComments();

        if (skipError.HasValue)
        {
            return skipError.Value;
        }

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

        if (c == '"')
        {
            return ScanString();
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
            '.' => MakeToken(TokenType.Dot),
            '[' => MakeToken(TokenType.LBracket),
            ']' => MakeToken(TokenType.RBracket),
            '(' => MakeToken(TokenType.LParen),
            ')' => MakeToken(TokenType.RParen),
            '{' => MakeToken(TokenType.LBrace),
            '}' => MakeToken(TokenType.RBrace),
            ',' => MakeToken(TokenType.Comma),
            ';' => MakeToken(TokenType.Semicolon),
            _ => MakeError($"Unexpected character: '{c}'"),
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

    private char Peek()
    {
        if (IsAtEnd)
        {
            return '\0';
        }

        return _source[_index];
    }

    private char PeekNext()
    {
        throw new NotImplementedException();
    }

    private char Advance()
    {
        char current = Peek();
        _index++;

        if (current == '\n')
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
        if (IsAtEnd || _source[_index] != expected)
        {
            return false;
        }

        Advance();
        return true;
    }

    private void SkipWhitespaceAndComments()
    {
        throw new NotImplementedException();
    }

    private Token ScanIdentifierOrKeyword()
    {
        while (IsAsciiLetterOrDigit(Peek()) || Peek() == '_')
        {
            Advance();
        }

        string text = _source.Substring(_tokenStartIndex, _index - _tokenStartIndex);

        if (Keywords.TryGetKeyword(text, out TokenType keywordType))
        {
            return MakeToken(keywordType);
        }

        return MakeToken(TokenType.Identifier);
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
        throw new NotImplementedException();
    }

    private Token MakeToken(TokenType type)
    {
        string lexeme = _source.Substring(_tokenStartIndex, _index - _tokenStartIndex);
        return new Token(type, lexeme, _tokenStart);
    }

    private Token MakeError(string message)
    {
        _faulted = true;
        return new Token(TokenType.Error, string.Empty, _tokenStart, message);
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