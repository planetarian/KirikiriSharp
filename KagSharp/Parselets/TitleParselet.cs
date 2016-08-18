using System;
using System.Collections.Generic;
using System.Text;
using KagSharp.Expressions;
using KagSharp.Lexer;

namespace KagSharp.Parselets
{
    public class TitleParselet : IPrefixParselet
    {
        public IExpression Parse(Parser parser, Token token)
        {
            string name = null;
            if (parser.LookAhead().IsType(TokenType.Text))
            {
                IExpression nameEx = parser.ParseExpression<TitleExpression>();
                name = ((TextExpression) nameEx).Text;
            }

            return new TitleExpression(name);
        }
    }
}
