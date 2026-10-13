using System;

namespace Nixie.Lexer;

public class LexerException : Exception
{
    public LexerException()
    {
    }

    public LexerException(string message)
        : base(message)
    {
    }

    public LexerException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public LexerException(string message, SourceLocation location)
        : base($"{message} at {location}")
    {
        Location = location;
    }

    public SourceLocation Location { get; }
}