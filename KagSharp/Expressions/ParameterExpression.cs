using System;
using System.Text;

namespace KagSharp.Expressions
{
    public class ParameterExpression : IdentifierExpression
    {
        public IExpression Value { get; }

        public ParameterExpression(string name, IExpression value) : base(name)
        {
            Value = value;
        }

        public override void Print(StringBuilder sb, bool verbose, int indentLevel)
        {
            sb.Append(Name);

            if (Value != null)
            {
                sb.Append("=");
                Value.Print(sb, verbose, 0);
            }
        }

        public override TR Accept<TR>(IExpressionVisitor<TR> visitor, string context) =>
            visitor.Visit(this, context);

        public override Type ValueType => Value.ValueType;

        public override string ToString() => "ParameterExpression: " + Name;
    }
}
