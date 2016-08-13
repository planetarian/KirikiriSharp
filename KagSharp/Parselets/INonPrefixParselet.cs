using KirikiriSharp.Expressions;
using KirikiriSharp.Lexer;

namespace KirikiriSharp.Parselets
{
    public interface INonPrefixParselet
    {
        IExpression Parse(Parser parser, IExpression left, Token token);
        int Precedence { get; }
        bool ConsumeToken { get; }
    }
}
