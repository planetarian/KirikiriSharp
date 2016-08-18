namespace KagSharp
{
    public static class PrecedenceValues
    {
        public const int End = 0;
        public const int Assignment = 1;
        public const int List = 2;
        public const int Compound = 3;
        public const int Ternary = 4;
        public const int Sum = 5;
        public const int Product = 6;
        public const int Exponent = 7;
        public const int Prefix = 8;
        public const int Postfix = 9;
        public const int Function = 10;
    }
}
