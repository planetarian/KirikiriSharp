namespace KagSharp.Lexer
{
    public interface ISourceReader
    {
        string Description { get; }
        char Current { get; }
        char Next { get; }
        void Advance();
    }
}
