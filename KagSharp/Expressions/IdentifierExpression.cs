using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace KagSharp.Expressions
{
    public class IdentifierExpression : IExpression
    {
        public string Name { get; }
        public string Value { get; }

        public static Dictionary<string, Type> RegisteredIdentifiers
            = new Dictionary<string, Type>();

        public IdentifierExpression(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public void Print(StringBuilder sb, bool verbose)
        {
            if (Regex.IsMatch(Value, @"^[\d\w]+$", RegexOptions.Compiled))
                sb.Append(Value);
            else
                sb.Append('"').Append(Value).Append('"');
        }

        public TR Accept<TR>(IExpressionVisitor<TR> visitor, string context) =>
            visitor.Visit(this, context);

        public Type ValueType => RegisteredIdentifiers.ContainsKey(Name)
            ? RegisteredIdentifiers[Name]
            : typeof(IdentifierExpression);

        public override string ToString() => "IdentifierExpression: " + Name;
    }
}
