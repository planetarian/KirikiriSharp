using System;
using System.Collections.Generic;
using System.Linq;
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
            IExpression nameEx = parser.ParseExpression<TagExpression>();
            string name = ((IdentifierExpression)nameEx).Name;

            var parameters = new List<IExpression>();

            while (parser.LookAhead().IsType(TokenType.Identifier))
            {
                IExpression param = parser.ParseExpression<TagExpression>();
                parameters.Add(param);
            }

            parser.Match(TokenType.RightBracket); // optional

            if (new[] {"if", "elsif", "else"}.Contains(name))
            {
                var children = new List<IExpression>();
                IfExpression fallback = null;
                TagExpression childTag;
                do
                {
                    IExpression child = parser.ParseExpression<IfExpression>();
                    childTag = child as TagExpression;
                    var ifChild = childTag as IfExpression;
                    if (childTag == null || childTag.Name != "endif" && (ifChild == null || ifChild.IsRoot))
                        children.Add(child);
                    else
                        fallback = ifChild;
                } while (fallback == null && childTag?.Name != "endif");
                return new IfExpression(name, parameters, children, fallback);
            }

            if (name == "macro")
            {
                var children = new List<IExpression>();
                TagExpression childTag;
                do
                {
                    IExpression child = parser.ParseExpression<IfExpression>();
                    childTag = child as TagExpression;
                    if (childTag == null || childTag.Name != "endmacro")
                        children.Add(child);
                } while (childTag?.Name != "endmacro");
                return new MacroExpression(name, parameters, children);
            }

            return new TagExpression(name, parameters);
        }
    }
}
