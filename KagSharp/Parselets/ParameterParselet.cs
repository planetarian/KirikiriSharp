using System;
using System.Collections.Generic;
using System.Text;
using KagSharp.Expressions;
using KagSharp.Lexer;

namespace KagSharp.Parselets
{
    public class ParameterParselet : INonPrefixParselet
    {
        public int Precedence => PrecedenceValues.Assignment;
        public bool ConsumeToken => true;
        

        public IExpression Parse(Parser parser, IExpression left, Token token)
        {
            if (!(left is IdentifierExpression)) throw new ParseException(
                token.Position, "The left-hand side of a parameter assignment must be an identifier.");
            string name = ((IdentifierExpression)left).Name;

            IExpression right = parser.ParseExpression<ParameterExpression>(Precedence - 1);

            return new ParameterExpression(name, right);
        }
    }
}
