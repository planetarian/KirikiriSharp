using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace KagSharp.Expressions
{
    public class IdentifierExpression : IExpression
    {
        public string Name { get; }

        public static Dictionary<string, Type> RegisteredIdentifiers
            = new Dictionary<string, Type>();

        public IdentifierExpression(string name)
        {
            Name = name;
        }

        public virtual void Print(StringBuilder sb, bool verbose, int indentLevel)
        {
            // Should never be starting a line with an identifier
            //ExpressionHelper.Indent(sb, GetType(), indentLevel);

            if (Regex.IsMatch(Name, @"^[\d\w]+$", RegexOptions.Compiled))
                sb.Append(Name);
            else
                sb.Append('"').Append(Name).Append('"');
        }

        public virtual TR Accept<TR>(IExpressionVisitor<TR> visitor, string context) =>
            visitor.Visit(this, context);

        public virtual Type ValueType => RegisteredIdentifiers.ContainsKey(Name)
            ? RegisteredIdentifiers[Name]
            : typeof(IdentifierExpression);

        public override string ToString() => "IdentifierExpression: " + Name;
    }
}
