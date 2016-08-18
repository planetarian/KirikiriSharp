using System;
using System.Collections.Generic;
using System.Text;

namespace KagSharp.Expressions
{
    public static class ExpressionHelper
    {
        private static Type _previousExpressionType;

        public static void Indent(StringBuilder sb, Type expressionType, int indentLevel)
        {
            if (_previousExpressionType == typeof (EndOfLineExpression))
                sb.Append("".PadLeft(indentLevel, '\t'));
            _previousExpressionType = expressionType;
        }

        public static void PrintDelimited<T>(
            StringBuilder sb, List<T> expressions, string delimiter, bool verbose, int indentLevel = 0) where T : IExpression
        {
            for (int i = 0; i < expressions.Count; i++)
            {
                expressions[i].Print(sb, verbose, indentLevel);
                if (i < expressions.Count - 1)
                    sb.Append(delimiter);
            }
        }
        public static void PrintDelimited<T>(
            StringBuilder sb, Dictionary<string, T> expressions, string delimiter, bool verbose, int indentLevel = 0)
            where T : IExpression
        {
            int i = 0;
            foreach (T expr in expressions.Values)
            {
                i++;
                expr.Print(sb, verbose, indentLevel);
                if (i < expressions.Count)
                    sb.Append(delimiter);
            }
        }

        public static void PrintDelimited<T>(
            StringBuilder sb, List<T> list, string delimiter)
        {
            for (int i = 0; i < list.Count; i++)
            {
                sb.Append(list[i]);
                if (i < list.Count - 1)
                    sb.Append(delimiter);
            }
        }

        public static Type GetMultiExpressionValueType(IEnumerable<Type> types)
        {
            bool hasFloat = false;
            bool hasString = false;
            bool hasList = false;

            foreach (Type type in types)
            {
                if (type == typeof(int))
                {
                }
                //hasInt = true;
                else if (type == typeof(float))
                    hasFloat = true;
                else if (type == typeof(string))
                    hasString = true;
                else if (type == typeof(List<>))
                    hasList = true;
                else
                    throw new InvalidOperationException(
                        "Can't perform operation with type " + type);
            }
            if (hasString)
                return typeof(string);
            if (hasList)
                return typeof(List<>);
            return hasFloat ? typeof(float) : typeof(int);
        }
    }
}
