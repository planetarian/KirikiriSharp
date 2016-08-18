using System;
using System.Collections.Generic;
using System.Text;
using KagSharp.Expressions;
using KagSharp.Lexer;

namespace KagSharp.Parselets
{
    public class TextParselet : IPrefixParselet
    {
        public IExpression Parse(Parser parser, Token token)
        {

            return new TextExpression(token.Text);
        }
    }
}
