using System;
using System.Text;

namespace KagSharp.Expressions
{
    public class EofExpression : IExpression
    {
        public void Print(StringBuilder sb, bool verbose)
        {
        }


        public TR Accept<TR>(IExpressionVisitor<TR> visitor, string context) =>
            visitor.Visit(this, context);

        public Type ValueType => typeof(EofExpression);

        public override string ToString() => "EofExpression";
    }
}
