using System.Diagnostics;

namespace KagSharp.Lexer
{
    public class Morpher : ITokenReader
    {
        private readonly ITokenReader _reader;
        //private bool eatLines;

        public Morpher(ITokenReader reader)
        {
            _reader = reader;
            //eatLines = true;
        }
        
        public Token ReadToken()
        {
            while (true)
            {
                Token token = _reader.ReadToken();
                switch (token.Type)
                {
                    case TokenType.WhiteSpace:
                    case TokenType.LineComment:
                    //case TokenType.LineEnd:
                        continue;
                    default:
                        return token;
                }
            }
        }
    }
}
