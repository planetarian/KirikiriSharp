using System;
using System.Text;

namespace KagSharp.Expressions
{
    public class ParameterExpression : IExpression
    {
        public string Name { get; }
        public IExpression Value { get; }

        public ParameterExpression(string name, IExpression value)
        {
            Name = name;
            Value = value;
        }

        public void Print(StringBuilder sb, bool verbose)
        {
            sb.Append(Name);

            if (Value != null)
            {
                sb.Append("=");
                Value.Print(sb, verbose);
            }
        }

        public TR Accept<TR>(IExpressionVisitor<TR> visitor, string context) =>
            visitor.Visit(this, context);

        public Type ValueType => Value.ValueType;

        public override string ToString() => "ParameterExpression: " + Name;
    }
}
