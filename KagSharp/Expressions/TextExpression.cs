using System;
using System.Text;

namespace KagSharp.Expressions
{
    public class TextExpression : IExpression
    {
        public string Text { get; }
        
        public TextExpression(string text)
        {
            Text = text;
        }

        public void Print(StringBuilder sb, bool verbose) => sb.Append(Text);

        public TR Accept<TR>(IExpressionVisitor<TR> visitor, string context) =>
            visitor.Visit(this, context);

        public Type ValueType => typeof(TextExpression);

        public override string ToString() => "TextExpression: " + Text;
    }
}
