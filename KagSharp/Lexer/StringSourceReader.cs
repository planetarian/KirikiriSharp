using KagSharp.Util;

namespace KagSharp.Lexer
{
    public class StringSourceReader : ISourceReader
    {
        public string Description { get; }
        public char Current => _position >= _text.Length ? '\0' : _text[_position];
        public char Next => _position + 1 >= _text.Length ? '\0' : _text[_position + 1];

        private readonly string _text;
        private int _position;

        public StringSourceReader(string description, string text)
        {
            Expect.NotEmpty(description, text);

            Description = description;
            _text = text;
            _position = 0;
        }
        
        public void Advance()
        {
            if (_position < _text.Length) _position++;
        }
    }
}
