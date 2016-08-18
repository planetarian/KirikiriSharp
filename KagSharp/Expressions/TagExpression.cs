using System;
using System.Collections.Generic;
using System.Text;

namespace KagSharp.Expressions
{
    public class TagExpression : IExpression
    {
        public string Name { get; }
        public List<IExpression> Parameters { get; }

        public TagExpression(string name, List<IExpression> parameters)
        {
            Name = name;
            Parameters = parameters;
        }

        public void Print(StringBuilder sb, bool verbose)
        {
            sb.Append('[').Append(Name);
            if (Parameters != null && Parameters.Count > 0)
            {
                sb.Append(' ');
                ExpressionHelper.PrintDelimited(sb, Parameters, " ", verbose);
            }
            sb.Append(']');
        }

        public TR Accept<TR>(IExpressionVisitor<TR> visitor, string context) => visitor.Visit(this, context);

        public Type ValueType => typeof(TagExpression);

        public override string ToString() => "TagExpression: " + Name;
    }
}
