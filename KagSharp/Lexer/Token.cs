using System.Text.RegularExpressions;
using KirikiriSharp.Util;

namespace KirikiriSharp.Lexer
{
    public class Token
    {
        public Position Position { get; private set; }
        public TokenType Type { get; }
        public string Text { get; }
        public object Value { get; private set; }

        public Token(Position position, TokenType type, string text, object value)
        {
            Expect.NotNull(position, type, text);

            Position = position;
            Type = type;
            Text = text;
            Value = value;
        }

        public bool IsType(TokenType type) => Type == type;

        public override string ToString() => $"({Type}) {Regex.Escape(Text)} : {Regex.Escape(Value.ToString())}";
    }
}
