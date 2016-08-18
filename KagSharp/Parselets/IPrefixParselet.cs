using KagSharp.Expressions;
using KagSharp.Lexer;

namespace KagSharp.Parselets
{
    public interface IPrefixParselet
    {
        IExpression Parse(Parser parser, Token token);
    }
}
