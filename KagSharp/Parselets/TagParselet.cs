using System;
using System.Collections.Generic;
using System.Text;
using KagSharp.Expressions;
using KagSharp.Lexer;

namespace KagSharp.Parselets
{
    public class TagParselet : IPrefixParselet
    {
        public IExpression Parse(Parser parser, Token token)
        {
            if (!parser.LookAhead().IsType(TokenType.Identifier))
                throw new ParseException(token.Position, "Tag name must be a valid identifier.");
            IExpression nameEx = parser.ParseExpression<LabelExpression>();
            string name = ((IdentifierExpression)nameEx).Name;

            var parameters = new List<IExpression>();

            while (parser.LookAhead().IsType(TokenType.Identifier))
            {
                IExpression param = parser.ParseExpression<TagExpression>();
                parameters.Add(param);
            }

            parser.Match(TokenType.RightBracket); // optional

            return new TagExpression(name, parameters);
        }
    }
}
