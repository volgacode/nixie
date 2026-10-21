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

    public LexerException(string message, SourcePosition Position)
        : base($"{message} at {Position}")
    {
        Position = Position;
    }

    public SourcePosition Position { get; }
}