using System;

namespace KagSharp
{
    public class ParseException : Exception
    {
        public Position Position { get; private set; }

        public ParseException(Position position, string message, Exception innerException = null)
            : base(message, innerException)
        {
            Position = position;
        }
    }
}
