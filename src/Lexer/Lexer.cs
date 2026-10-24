namespace Nixie.Lexer;

public class Lexer
{
    private readonly string _source;
    private int _index;
    private SourcePosition _position;
    private SourcePosition _tokenStart;
    private bool _faulted;

    public Lexer(string source)
    {
        _source = source;
        _index = 0;
        _position = new SourcePosition(1, 1);
        _tokenStart = _position;
        _faulted = false;
    }

    public Token NextToken()
    {
        throw new NotImplementedException();
    }

    public List<Token> TokenizeAll()
    {
        throw new NotImplementedException();
    }

    private bool IsAtEnd => _index >= _source.Length;

    private char Peek()
    {
        throw new NotImplementedException();
    }

    private char PeekNext()
    {
        throw new NotImplementedException();
    }

    private char Advance()
    {
        throw new NotImplementedException();
    }

    private bool Match(char expected)
    {
        throw new NotImplementedException();
    }

    private void SkipWhitespaceAndComments()
    {
        throw new NotImplementedException();
    }

    private Token ScanIdentifierOrKeyword()
    {
        throw new NotImplementedException();
    }

    private Token ScanNumber()
    {
        throw new NotImplementedException();
    }

    private Token ScanString()
    {
        throw new NotImplementedException();
    }

    private Token MakeToken(TokenType type)
    {
        throw new NotImplementedException();
    }

    private Token MakeError(string message)
    {
        throw new NotImplementedException();
    }

    private static bool IsAsciiLetter(char c)
    {
        throw new NotImplementedException();
    }

    private static bool IsAsciiDigit(char c)
    {
        throw new NotImplementedException();
    }

    private static bool IsAsciiLetterOrDigit(char c)
    {
        throw new NotImplementedException();
    }
}