using System;

namespace KirikiriSharp
{
    public class ParseException : Exception
    {
        public Position Position { get; private set; }
        public ParseException(Position position, String message)
            : base(message)
        {
            Position = position;
        }
    }
}
