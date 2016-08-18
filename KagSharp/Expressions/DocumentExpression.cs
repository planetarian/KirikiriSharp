using System;
using System.Collections.Generic;
using System.Text;

namespace KagSharp.Expressions
{
    public class DocumentExpression : IExpression
    {
        public List<IExpression> Children { get; }

        public DocumentExpression(List<IExpression> children)
        {
            Children = children;
        }

        public void Print(StringBuilder sb, bool verbose, int indentLevel)
        {
            ExpressionHelper.Indent(sb, GetType(), indentLevel);
            ExpressionHelper.PrintDelimited(sb, Children, "", verbose);
        }

        public TR Accept<TR>(IExpressionVisitor<TR> visitor, string context) =>
            visitor.Visit(this, context);

        public Type ValueType => typeof(DocumentExpression);

        public override string ToString() => "DocumentExpression";
    }
}
