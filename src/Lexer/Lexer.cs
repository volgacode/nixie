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
        throw new NotImplementedException();
    }

    public List<Token> TokenizeAll()
    {
        throw new NotImplementedException();
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