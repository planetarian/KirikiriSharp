using System;
using System.Collections.Generic;
using System.Text;

namespace KagSharp.Expressions
{
    public class TagExpression : IdentifierExpression
    {
        public List<IExpression> Parameters { get; }

        public TagExpression(string tagName, List<IExpression> parameters) : base(tagName)
        {
            Parameters = parameters;
        }

        public override void Print(StringBuilder sb, bool verbose, int indentLevel)
        {
            ExpressionHelper.Indent(sb, GetType(), indentLevel);
            sb.Append('[').Append(Name);
            if (Parameters != null && Parameters.Count > 0)
            {
                sb.Append(' ');
                ExpressionHelper.PrintDelimited(sb, Parameters, " ", verbose);
            }
            sb.Append(']');
        }

        public override TR Accept<TR>(IExpressionVisitor<TR> visitor, string context) =>
            visitor.Visit(this, context);

        public override Type ValueType => typeof(TagExpression);

        public override string ToString() => "TagExpression: " + Name;
    }
}
