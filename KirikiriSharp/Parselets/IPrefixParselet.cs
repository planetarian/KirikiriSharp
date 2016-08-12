using System;
using KirikiriSharp.Expressions;
using KirikiriSharp.Lexer;

namespace KirikiriSharp.Parselets
{
    public interface IPrefixParselet
    {
        IExpression Parse(Parser parser, Token token);
    }
}
