using System;
using System.Collections.Generic;
using System.Text;
using KagSharp.Expressions;
using KagSharp.Lexer;

namespace KagSharp.Parselets
{
    public class IdentifierParselet : IPrefixParselet
    {
        public IExpression Parse(Parser parser, Token token) =>
            new IdentifierExpression(token.Value.ToString());
    }
}
