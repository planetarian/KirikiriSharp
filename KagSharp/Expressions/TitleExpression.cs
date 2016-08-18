using System;
using System.Text;

namespace KagSharp.Expressions
{
    public class TitleExpression : IExpression
    {
        public string Name { get; }

        public TitleExpression(string name)
        {
            Name = name;
        }

        public void Print(StringBuilder sb, bool verbose, int indentLevel)
        {
            ExpressionHelper.Indent(sb, GetType(), indentLevel);
            sb.Append('#').Append(Name);
        }

        public TR Accept<TR>(IExpressionVisitor<TR> visitor, string context) =>
            visitor.Visit(this, context);

        public Type ValueType => typeof(TitleExpression);

        public override string ToString() => "TitleExpression: " + Name;
    }
}
