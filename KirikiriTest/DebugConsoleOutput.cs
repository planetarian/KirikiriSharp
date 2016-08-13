using System;
using Tjs2.Engine;

namespace KirikiriTest
{

    public class DebugConsoleOutput : IConsoleOutput
    {
        public void ExceptionPrint(string msg)
        {
            Console.WriteLine(@"Error: " + msg);
        }

        public void Print(string msg)
        {
            Console.WriteLine(@"OUT: " + msg);
        }
    }
}
