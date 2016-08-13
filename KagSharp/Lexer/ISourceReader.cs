namespace KirikiriSharp.Lexer
{
    public interface ISourceReader
    {
        string Description { get; }
        char Current { get; }
        void Advance();
    }
}
