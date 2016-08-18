using System;

namespace KagSharp
{
    public class Position
    {
        public string SourceFile { get; }
        public int StartLine { get; }
        public int StartCol { get; }
        public int EndLine { get; }
        public int EndCol { get; }

        public static Position None => new Position(String.Empty, -1, -1, -1, -1);

        public Position(string sourceFile, int startLine, int startCol, int endLine, int endCol)
        {
            SourceFile = sourceFile;
            StartLine = startLine;
            StartCol = startCol;
            EndLine = endLine;
            EndCol = endCol;
        }

        public Position Union(Position other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));
            if (other.SourceFile != SourceFile)
                throw new InvalidOperationException(
                    "Tried to Union Position objects but source files do not match.");
            int startLine = Math.Min(StartLine, other.StartLine);
            int startCol = Math.Min(StartCol, other.StartCol);
            int endLine = Math.Max(EndLine, other.EndLine);
            int endCol = Math.Max(EndCol, other.EndCol);
            return new Position(SourceFile, startLine, startCol, endLine, endCol);
        }

        public static Position Union(Position pos1, Position pos2)
        {
            if (pos1 == null || pos2 == null)
                throw new ArgumentNullException(pos1 == null ? nameof(pos1) : nameof(pos2));
            if (pos1.SourceFile != pos2.SourceFile)
                throw new InvalidOperationException(
                    "Tried to Union Position objects but source files do not match.");
            int startLine = Math.Min(pos1.StartLine, pos2.StartLine);
            int startCol = Math.Min(pos1.StartCol, pos2.StartCol);
            int endLine = Math.Max(pos1.EndLine, pos2.EndLine);
            int endCol = Math.Max(pos1.EndCol, pos2.EndCol);
            return new Position(pos1.SourceFile, startLine, startCol, endLine, endCol);
        }

        public override string ToString()
        {
            if (StartLine == -1)
                return "(Unknown position)";
            return StartLine == EndLine
                ? $"{SourceFile} (line {StartLine}, col {StartCol}-{EndCol})"
                : $"{SourceFile} (line {StartLine} col {StartCol} - line {EndLine} col {EndCol})";
        }
    }
}
