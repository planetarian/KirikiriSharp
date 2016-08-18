using KagSharp.Expressions;
using KagSharp.Lexer;

namespace KagSharp.Parselets
{
    public interface INonPrefixParselet
    {
        IExpression Parse(Parser parser, IExpression left, Token token);
        int Precedence { get; }
        bool ConsumeToken { get; }
    }
}
