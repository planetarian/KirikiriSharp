using System;
using System.Collections.Generic;
using System.Text;
using KirikiriSharp.Util;

namespace KirikiriSharp.Lexer
{
    public class Lexer : ITokenReader
    {
        private readonly ISourceReader _reader;
        private string _read;
        private int _startLine;
        private int _startCol;
        private int _line;
        private int _col;
        private char _cur;
        private char _prev;

        private static Dictionary<string, TokenType> Keywords { get; }
            = new Dictionary<string, TokenType>();

        private char Current => _reader.Current;

        private Position CurrentPosition => new Position(_reader.Description, _startLine, _startCol, _line, _col);

        static Lexer()
        {
            Keywords.Add("*", TokenType.Asterisk);
            Keywords.Add("|", TokenType.Pipe);
            Keywords.Add("@", TokenType.At);
            Keywords.Add("[", TokenType.LeftBracket);
            Keywords.Add("]", TokenType.RightBracket);
            /*
            Keywords.Add("global", TokenType.Global);
            Keywords.Add("scene", TokenType.Scene);
            Keywords.Add("template", TokenType.Template);
            Keywords.Add("entity", TokenType.Entity);
            Keywords.Add("group", TokenType.Group);
            Keywords.Add("eval", TokenType.Eval);
            Keywords.Add("true", TokenType.Boolean);
            Keywords.Add("false", TokenType.Boolean);
            Keywords.Add("?", TokenType.Question);
            Keywords.Add("(", TokenType.LeftParen);
            Keywords.Add(")", TokenType.RightParen);
            Keywords.Add("{", TokenType.LeftBrace);
            Keywords.Add("}", TokenType.RightBrace);
            Keywords.Add(",", TokenType.Comma);
            Keywords.Add(":", TokenType.Colon);
            Keywords.Add(";", TokenType.SemiColon);
            Keywords.Add("*", TokenType.Asterisk);
            Keywords.Add("/", TokenType.Slash);
            Keywords.Add("%", TokenType.Percent);
            Keywords.Add("+", TokenType.Plus);
            Keywords.Add("-", TokenType.Minus);
            Keywords.Add("^", TokenType.Caret);
            */
        }

        public Lexer(ISourceReader reader)
        {
            Expect.NotNull(reader);

            _reader = reader;
            _line = 1;
            _col = 1;
            _startLine = 1;
            _startCol = 1;
            _read = String.Empty;
        }

        public Token ReadToken()
        {
            _prev = _cur;
            _cur = Advance();
            switch (_cur)
            {
                case ' ':
                case '\t':
                    return ReadWhitespace();
                case '\r':
                case '\n':
                    if (_cur == '\r' && Current == '\n')
                        Advance();
                    return MakeToken(TokenType.LineEnd);
                case '*':
                    return MakeToken(TokenType.Asterisk);

                    /* TODO
                case '(':
                    return MakeToken(TokenType.LeftParen);
                case ')':
                    return MakeToken(TokenType.RightParen);
                case '{':
                    return MakeToken(TokenType.LeftBrace);
                case '}':
                    return MakeToken(TokenType.RightBrace);
                case ',':
                    return MakeToken(TokenType.Comma);
                case ';':
                    return MakeToken(TokenType.SemiColon);
                case ':':
                    return MakeToken(TokenType.Colon);
                case '?':
                    return MakeToken(TokenType.Question);
                case '!':
                    return MakeToken(TokenType.Bang);
                case '@':
                    return MakeToken(TokenType.At);
                case '"':
                    return ReadString();
                case '/':
                    switch (Current)
                    {
                        case '/':
                            return ReadLineComment();
                        case '*':
                            return ReadBlockComment();
                        default:
                            return ReadIdentifier();
                    }*/

                case '-':
                case '.':
                    if (_cur == '.' && IsDigit(Current))
                        return ReadNumber();
                    if (_cur == '-' && IsDigit(Current) && !IsDigit(_prev))
                        return ReadNumber();
                    return IsOperator(Current)
                        ? ReadIdentifier()
                        : MakeToken(TokenType.Identifier);

                case '\0':
                    return MakeToken(TokenType.Eof);

                default:
                    if (IsIdentifier(_cur))
                        return ReadIdentifier();
                    //if (IsOperator(_cur))
                        //return ReadOperator();
                    if (IsDigit(_cur))
                        return ReadNumber();

                    throw new ParseException(Position.None, "Unknown character.");
            }
        }

        private Token ReadWhitespace()
        {
            while (true)
            {
                switch (Current)
                {
                    case ' ':
                    case '\t':
                        Advance();
                        break;
                    default:
                        return MakeToken(TokenType.WhiteSpace);
                }
            }
        }

        /* TODO: private Token ReadString()
        {
            var escaped = new StringBuilder();
            while (true)
            {

                char c = Advance();
                switch (c)
                {
                    case '\\':
                        // escape sequence
                        char e = Advance();
                        switch (e)
                        {
                            case '\'':
                                escaped.Append('\'');
                                break;
                            case '"':
                                escaped.Append('\"');
                                break;
                            case '\\':
                                escaped.Append('\\');
                                break;
                            case '0':
                                escaped.Append('\0');
                                break;
                            case 'a':
                                escaped.Append('\a');
                                break;
                            case 'b':
                                escaped.Append('\b');
                                break;
                            case 'f':
                                escaped.Append('\f');
                                break;
                            case 'n':
                                escaped.Append('\n');
                                break;
                            case 'r':
                                escaped.Append('\r');
                                break;
                            case 't':
                                escaped.Append('\t');
                                break;
                            case 'v':
                                escaped.Append('\v');
                                break;
                            //TODO case 'U'
                            //case 'U':
                                //escaped.Append('\U');
                                //break;
                            case 'u':
                            case 'x':
                                int a = ReadHexDigit();
                                int b = ReadHexDigit();
                                int code = (a << 4) | b;
                                //TODO: 4-digit codes too
                                escaped.Append((char)code);
                                break;
                            default:
                                // TODO: error token
                                throw new ParseException(Position.None, "Unknown string escape sequence.");
                        }
                        break;
                    case '"':
                        return MakeToken(TokenType.String, escaped.ToString());
                    case '\0':
                        // TODO: error token
                        throw new ParseException(Position.None, "Unexpected end of file within string value.");
                    default:
                        escaped.Append(c);
                        break;
                }

            }
        }*/

        private int ReadHexDigit()
        {
            char c = char.ToLower(Advance());
            int digit = "0123456789abcdef".IndexOf(c);
            if (digit == -1)
            {
                // TODO: error token instead of exception
                throw new ParseException(Position.None, "Expected hex digit.");
            }
            return digit;
        }

        /* TODO private Token ReadLineComment()
        {
            Advance(); // second '/'

            int slashCount = 2;
            
            while (Current == '/')
            {
                ++slashCount;
                Advance();
            }

            while (true)
            {
                switch (Current)
                {
                    case '\n':
                    case '\r':
                    case '\0':
                        string value = _read.Substring(slashCount).Trim();
                        return MakeToken(TokenType.LineComment, value);
                    default:
                        Advance();
                        break;
                }
            }

        }*/

        /* TODO private Token ReadBlockComment()
        {
            const string nullError = "Unexpected end of file inside block comment.";
            while (true)
            {
                switch(Advance())
                {
                    case '*':
                        switch (Advance())
                        {
                            case '/':
                                return MakeToken(TokenType.BlockComment);
                            case '\0':
                                // TODO: Emit error token instead of exception
                                throw new ParseException(Position.None, nullError);
                                // Otherwise keep advancing.
                        }
                        break;
                    case '\0':
                        // TODO: Emit error token instead of exception
                        throw new ParseException(Position.None, nullError);
                    // Otherwise keep advancing.
                }
            }
        }*/

        private Token ReadIdentifier()
        {
            int idx = 0;
            while (true)
            {
                if (IsIdentifier(Current) || (idx > 0 && IsDigit(Current)))
                    Advance();
                else
                    return MakeToken(TokenType.Identifier);
                idx++;
            }
        }

        private Token ReadOperator()
        {
            while (true)
            {
                if (IsIdentifier(Current) || IsOperator(Current))
                    Advance();
                else
                    return MakeToken(TokenType.Identifier);
            }
        }


        private Token ReadNumber()
        {
            int periods = _read[0] == '.' ? 1 : 0;
            while (true)
            {
                char c = Current;
                if (c == '.')
                {
                    periods++;
                    if (periods > 1)
                        throw new ParseException(Position.None, "Invalid decimal value.");
                    Advance();
                }
                else if (IsDigit(c))
                {
                    Advance();
                }
                else
                {
                    return periods > 0
                        ? MakeToken(TokenType.Float, float.Parse(_read))
                        : MakeToken(TokenType.Integer, Int32.Parse(_read));
                }
            }
        }

        private char Advance()
        {
            char c = Current;
            _reader.Advance();
            _read += c;

            // update position
            if (c == '\n')
            {
                _line++;
                _col = 1;
            }
            else _col++;

            return c;
        }

        private Token MakeToken(TokenType type)
        {
            return MakeToken(type, _read);
        }

        private Token MakeToken(TokenType type, object value)
        {
            Expect.NotEmpty(_read);
            string readLower = _read.ToLower();
            if (type == TokenType.Identifier)
            {
                if (Keywords.ContainsKey(readLower))
                {
                    type = Keywords[readLower];
                }

                if (readLower == "true")
                    value = true;
                else if (readLower == "false")
                    value = false;
            }

            var token = new Token(CurrentPosition, type, _read, value);

            _startLine = _line;
            _startCol = _col;
            _read = String.Empty;

            return token;
        }

        private static bool IsDigit(char c)
        {
            return (c >= '0') && (c <= '9');
        }

        private static bool IsIdentifier(char c)
        {
            return ((c >= 'a') && (c <= 'z'))
                || ((c >= 'A') && (c <= 'Z'))
                || (c == '_');//TODO: || (c == '.');
        }

        private static bool IsOperator(char c)
        {
            return "%*/-+".IndexOf(c) != -1;
        }

        public TokenType TypeFromKeyword(string keyword)
        {
            return Keywords.ContainsKey(keyword) ? Keywords[keyword] : TokenType.Invalid;
        }
    }
}
