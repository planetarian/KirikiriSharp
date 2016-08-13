using System;
using System.Text;

namespace KirikiriSharp.Expressions
{
    public class EofExpression : IExpression
    {
        public void Print(StringBuilder sb, bool verbose)
        {
        }

        public Type ValueType => typeof(EofExpression);

        public TR Accept<TR>(IExpressionVisitor<TR> visitor, string context)
        {
            return visitor.Visit(this, context);
        }
    }
}
