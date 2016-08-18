using System;
using System.Text;

namespace KagSharp.Expressions
{
    public interface IExpression
    {
        void Print(StringBuilder sb, bool verbose);
        TR Accept<TR>(IExpressionVisitor<TR> visitor, string context = null);
        Type ValueType { get; }
    }
}
