using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KagSharp.Expressions
{
    public class MacroExpression : TagExpression
    {
        public string MacroName { get; }
        public List<IExpression> Children { get; }

        public MacroExpression(
            string tagName, List<IExpression> parameters,
            List<IExpression> children)
            : base(tagName, parameters)
        {
            Children = children;
            MacroName = parameters
                .Where(p => p.ValueType==typeof(ParameterExpression))
                .Cast<ParameterExpression>()
                .FirstOrDefault(p => p.Name == "name")?
                .Value.ToString();
        }

        public override void Print(StringBuilder sb, bool verbose, int indentLevel)
        {
            ExpressionHelper.Indent(sb, GetType(), indentLevel);
            sb.Append("[macro");
            if (Parameters != null && Parameters.Count > 0)
            {
                sb.Append(' ');
                ExpressionHelper.PrintDelimited(sb, Parameters, " ", verbose);
            }
            sb.Append("]");
            ExpressionHelper.PrintDelimited(sb, Children, "", verbose, ++indentLevel);
            ExpressionHelper.Indent(sb, GetType(), --indentLevel);
            sb.Append("[endmacro]");
        }

        public override TR Accept<TR>(IExpressionVisitor<TR> visitor, string context) =>
            visitor.Visit(this, context);

        public override Type ValueType => typeof (TagExpression);

        public override string ToString() => "MacroExpression: " + MacroName;
    }
}
