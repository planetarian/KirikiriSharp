using System;
using System.Collections.Generic;
using System.Text;
using KagSharp.Expressions;
using KagSharp.Lexer;

namespace KagSharp.Parselets
{
    public class LabelParselet : IPrefixParselet
    {
        public IExpression Parse(Parser parser, Token token)
        {
            IExpression nameEx = parser.ParseExpression<LabelExpression>();
            if (nameEx.ValueType != typeof(IdentifierExpression))
                throw new ParseException(token.Position, "Label name must be a valid identifier.");
            string name = ((IdentifierExpression)nameEx).Name;

            string desc = name;
            if (parser.Match(TokenType.Pipe))
            {
                IExpression descEx = parser.ParseExpression<LabelExpression>();
                if (descEx.ValueType != typeof(TextExpression))
                    throw new ParseException(token.Position, "Label description must follow pipe.");
                desc = ((TextExpression) descEx).Text;
            }

            return new LabelExpression(name, desc);
        }
    }
}
