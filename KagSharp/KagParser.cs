using System.Collections.Generic;
using KagSharp.Expressions;
using KagSharp.Lexer;
using KagSharp.Parselets;

namespace KagSharp
{
    public class KagParser : Parser
    {
        public KagParser(ITokenReader reader) : base(reader)
        {
            Register(TokenType.Asterisk, new LabelParselet());
            Register(TokenType.Identifier, new IdentifierParselet());
            Register(TokenType.Text, new TextParselet());
            Register(TokenType.LeftBracket, new TagParselet());
            Register(TokenType.Equals, new ParameterParselet());
            Register(TokenType.At, new TagParselet());
            Register(TokenType.Hash, new TitleParselet());
            Register(TokenType.LineEnd, new EndOfLineParselet());
        }

        public DocumentExpression ParseDocument()
        {
            var children = new List<IExpression>();
            IExpression expression;
            while (!((expression = ParseExpression<DocumentExpression>()) is EofExpression))
            {
                children.Add(expression);
            }
            return new DocumentExpression(children);
        }
    }
}
