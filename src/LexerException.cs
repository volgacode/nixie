using System;

namespace Nixie.Lexer;

public class LexerException : Exception
{
    public SourceLocation Location { get; }

    public LexerException(string message, SourceLocation location)
        : base($"{message} at {location}")
    {
        Location = location;
    }
}