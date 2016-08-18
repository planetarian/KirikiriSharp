using System;
using System.Collections.Generic;
using System.Text;

namespace KagSharp.Expressions
{
    public class EndOfLineExpression : IExpression
    {
        public void Print(StringBuilder sb, bool verbose, int indentLevel)
        {
            ExpressionHelper.Indent(sb, GetType(), indentLevel);
            sb.Append('\n');
        }

        public TR Accept<TR>(IExpressionVisitor<TR> visitor, string context = null) =>
            visitor.Visit(this, context);

        public Type ValueType => typeof(EndOfLineExpression);

        public override string ToString() => "EndOfLineExpression";
    }
}
