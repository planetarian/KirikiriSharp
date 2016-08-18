using System;
using System.Collections.Generic;
using System.Text;

namespace KagSharp.Expressions
{
    public class LabelExpression : IExpression
    {
        public string Name { get; }
        public string Description { get; }

        public LabelExpression(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public void Print(StringBuilder sb, bool verbose)
        {
            sb.Append('*').Append(Name);
            if (!String.IsNullOrEmpty(Description) && Name != Description)
                sb.Append('|').Append(Description);
        }

        public TR Accept<TR>(IExpressionVisitor<TR> visitor, string context) => visitor.Visit(this, context);

        public Type ValueType => typeof(LabelExpression);

        public override string ToString() => "LabelExpression: " + Name;
    }
}
