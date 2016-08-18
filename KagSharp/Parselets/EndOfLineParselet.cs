using System;
using System.Collections.Generic;
using System.Text;
using KagSharp.Expressions;
using KagSharp.Lexer;

namespace KagSharp.Parselets
{
    public class EndOfLineParselet : IPrefixParselet
    {
        public IExpression Parse(Parser parser, Token token)
        {
            //while (parser.Match(TokenType.LineEnd))
            //{
            //}
            return new EndOfLineExpression();
        }
    }
}
