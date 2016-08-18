using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KagSharp.Expressions
{
    public class IfExpression : TagExpression
    {
        public IdentifierExpression Condition { get; }
        public List<IExpression> Children { get; }
        public IfExpression Fallback { get; }
        public bool IsRoot { get; }

        public IfExpression(
            string tagName, List<IExpression> parameters,
            List<IExpression> children, IfExpression fallback)
            : base(tagName, parameters)
        {
            IsRoot = tagName == "if";
            Children = children;
            Fallback = fallback;
            Condition = parameters
                .Where(p => p is ParameterExpression)
                .Cast<ParameterExpression>()
                .FirstOrDefault(p => p.Name == "exp")?
                .Value as IdentifierExpression;
        }

        public override void Print(StringBuilder sb, bool verbose, int indentLevel)
        {
            ExpressionHelper.Indent(sb, GetType(), indentLevel);
            sb.Append("[").Append(IsRoot ? "if" : (Condition != null ? "elsif" : "else"));
            if (Parameters != null && Parameters.Count > 0)
            {
                sb.Append(' ');
                ExpressionHelper.PrintDelimited(sb, Parameters, " ", verbose);
            }
            sb.Append("]");
            ExpressionHelper.PrintDelimited(sb, Children, "", verbose, ++indentLevel);
            indentLevel--;
            Fallback?.Print(sb, verbose, indentLevel);
            if (IsRoot)
            {
                ExpressionHelper.Indent(sb, GetType(), indentLevel);
                sb.Append("[endif]");
            }
        }

        public override TR Accept<TR>(IExpressionVisitor<TR> visitor, string context) =>
            visitor.Visit(this, context);

        public override Type ValueType => typeof(TagExpression);

        public override string ToString() => "IfExpression: " + Condition;
    }
}
