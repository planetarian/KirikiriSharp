using System;
using System.Linq;

namespace KagSharp.Util
{
    public static class Expect
    {
        public static T CanCast<T>(object value)
        {
            if (value.GetType() != typeof(T))
                throw new InvalidOperationException("Value must be of type " + typeof(T));
            return (T)value;
        }

        public static void IsEqual<T>(T var1, T var2)
        {
            NotNull(var1, var2);
            if (!var1.Equals(var2))
                throw new InvalidOperationException("Values must match.");
        }

        public static void NotNull(params object[] args)
        {
            if (args.Any(a => a == null))
                throw new InvalidOperationException("Value cannot be null.");
        }

        public static T NotNull<T>(T arg) where T : class
        {
            if (arg == null)
                throw new InvalidOperationException("Value cannot be null.");
            return arg;
        }

        public static void IsNull<T>(T arg) where T : class
        {
            if (arg != null)
                throw new InvalidOperationException("Value must be null.");
        }

        public static void NotEmpty(params string[] args)
        {
            foreach (string t in args)
            {
                NotNull(t);
                if (t == String.Empty)
                    throw new InvalidOperationException("String cannot be empty.");
            }
        }

        public static string NotEmpty(string arg)
        {
            NotNull(arg);
            if (arg == String.Empty)
                throw new InvalidOperationException("String cannot be empty.");
            return arg;
        }

        public static T NonNegative<T>(T arg)
            where T : struct, IComparable, IFormattable // numeric
        {
            if (arg.CompareTo(0) < 0)
                throw new InvalidOperationException("Number must be Non-negative.");
            return arg;
        }

        public static T PositiveNonZero<T>(T arg)
            where T : struct, IComparable, IFormattable // numeric
        {
            if (arg.CompareTo(0) <= 0)
                throw new InvalidOperationException("Number must be greater than zero.");
            return arg;
        }

        public static T NotZero<T>(T arg)
            where T : struct, IComparable, IFormattable // numeric
        {
            if (arg.CompareTo(0) == 0)
                throw new InvalidOperationException("Number cannot be zero.");
            return arg;
        }

    }
}
