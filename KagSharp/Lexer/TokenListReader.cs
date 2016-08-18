using System.Collections.Generic;

namespace KagSharp.Lexer
{
    public class TokenListReader : ITokenReader
    {
        private readonly List<Token> _tokens;
        private int _index;

        public TokenListReader(List<Token> tokens)
        {
            _tokens = tokens;
        }

        public Token ReadToken() => _index >= _tokens.Count
                ? new Token(Position.None, TokenType.Eof, "\0", null)
                : _tokens[_index++];
    }
}
