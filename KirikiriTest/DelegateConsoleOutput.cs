using System;
using Tjs2.Engine;

namespace KirikiriTest
{
    public class DelegateConsoleOutput : IConsoleOutput
    {
        private readonly Action<string> _printAction;
        private readonly Action<string> _errorAction;

        public DelegateConsoleOutput(Action<string> printAction, Action<string> errorAction = null)
        {
            _printAction = printAction;
            _errorAction = errorAction ?? printAction;
        }

        public void ExceptionPrint(string msg)
        {
            _errorAction(msg);
        }

        public void Print(string msg)
        {
            _printAction(msg);
        }
    }
}
