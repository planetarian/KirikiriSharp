using System;
using System.Collections.Generic;
using System.Linq;
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

        private bool _insideTitle;
        private bool _insideLabel;
        private bool _insideTag;
        private bool _inlineTag;

        private readonly char[] _eolChars = {'\r', '\n', '\0'};

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

        private char Advance()
        {
            char c = Current;
            _reader.Advance();
            _read += c;

            // update position
            if (c == '\n' || (c == '\r' && Current != '\n'))
            {
                _line++;
                _col = 1;
            }
            else _col++;

            return c;
        }

        private char Consume()
        {
            _prev = _cur;
            _cur = Advance();
            return _cur;
        }

        private char Skip()
        {
            _cur = Advance();
            return _cur;
        }

        public Token ReadToken()
        {
            Consume();
            while (true)
            {
                // Ignore tabs for free indentation
                if (_cur == '\t' || _cur == ' ')
                {
                    // remove whitespace character from read cache
                    _read = _read.Substring(0, _read.Length - 1);
                    Skip();
                    continue;
                }

                // Certain tokens only valid after a newline/start-of-file
                if (_eolChars.Contains(_prev))
                {
                    switch (_cur)
                    {
                        case '*':
                            _insideLabel = true;
                            return MakeToken(TokenType.Asterisk);
                        case '#':
                            if (!_eolChars.Contains(Current))
                                _insideTitle = true;
                            return MakeToken(TokenType.Hash);
                        case ';':
                            return ReadLineComment();
                        case '@':
                            _insideTag = true;
                            _inlineTag = false;
                            return MakeToken(TokenType.At);
                    } // switch _cur
                }

                if (_insideTitle){
                    return ReadText();}

                if (_insideLabel)
                {
                    switch (_cur)
                    {
                        case '|':
                            _insideLabel = false;
                            return MakeToken(TokenType.Pipe);
                        default:
                            return ReadIdentifier();
                    }
                }

                // Any other tokens, and non-post-newline tokens
                if (_insideTag)
                {
                    if (_inlineTag && _cur == ']')
                    {
                        _insideTag = false;
                        return MakeToken(TokenType.RightBracket);
                    }
                    switch (_cur)
                    {
                        case '=':
                            return MakeToken(TokenType.Equals);
                        default:
                            return ReadIdentifier();
                    }
                }

                switch (_cur)
                {
                    // newlines
                    case '\r': // mac
                        if (Current == '\n') // windows
                            Advance();
                        goto case '\n';
                    case '\n': // *nix
                        _insideTag = false;
                        return MakeToken(TokenType.LineEnd);
                    // eof
                    case '\0':
                        _insideTag = false;
                        return MakeToken(TokenType.Eof);
                    // tag
                    case '[':
                        if (Current != '[') // escaped LBracket counts as plain text
                        {
                            _insideTag = true;
                            _inlineTag = true;
                            return MakeToken(TokenType.LeftBracket);
                        }
                        goto default;
                    default:
                        return ReadText();
                }
            } // while true
        } // ReadToken()

        private Token ReadLineComment()
        {
            while (true)
            {
                switch (Current)
                {
                    case '\n':
                    case '\r':
                    case '\0':
                        string value = _read.Substring(1).Trim();
                        return MakeToken(TokenType.LineComment, value);
                    default:
                        Advance();
                        break;
                }
            }

        }//TODO*/

        private Token ReadText()
        {
            _insideTitle = false;
            var sb = new StringBuilder();
            char c = _cur;

            for (int i = 0; i < 9000; i++)
            {
                sb.Append(c);
                if (Current == '[')
                {
                    if (_reader.Next == '[')
                        Advance();
                    else
                        return MakeToken(TokenType.Text, sb.ToString());
                }

                if (_eolChars.Contains(Current))
                    return MakeToken(TokenType.Text, sb.ToString());
                c = Advance();
            }
            throw new ParseException(CurrentPosition, "ReadText exceeded maximum iterations.");

        }


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

        /* TODO private Token ReadWhitespace()
        {
            while (true)
            {
                switch (Current)
                {
                    case '\t':
                        Advance(); // tabs are ignored
                        break;
                    default:
                        return MakeToken(TokenType.WhiteSpace);
                }
            }
        } //*/

        /* TODO private Token ReadString()
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
        }//*/

        /* TODO private int ReadHexDigit()
        {
            char c = char.ToLower(Advance());
            int digit = "0123456789abcdef".IndexOf(c);
            if (digit == -1)
            {
                // TODO: error token instead of exception
                throw new ParseException(Position.None, "Expected hex digit.");
            }
            return digit;
        }//*/

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
        }//*/

        private Token ReadIdentifier()
        {
            var sb = new StringBuilder();
            char c = _cur;

            bool openQuotes = false;
            char quoteType = '"';
            int openBrackets = 0;
            int maxOpenBrackets = 0;


            for (int i = 0; i < 9000; i++)
            {
                bool skip = false;
                
                // whitespace & quotes
                bool isQuotedSpace = openQuotes && c == ' ';
                if (isQuotedSpace)
                    skip = true;

                else if ((c == '"' || c == '\'') && (!openQuotes || c == quoteType))
                {
                    if (!openQuotes)
                        quoteType = c;
                    openQuotes = !openQuotes;
                    skip = true;
                }

                else if (c == '[')
                {
                    openBrackets++;
                    if (openBrackets > maxOpenBrackets)
                        maxOpenBrackets = openBrackets;
                }

                else if (c == ']')
                {
                    // doesn't seem like this is a problem... consider warning about this in a 'strict' mode
                    //if (openBrackets <= 0 && openQuotes)
                        //throw new ParseException(CurrentPosition,
                            //"Don't use an RBracket within a quoted identifier without first using a LBracket.");
                            
                    if (openBrackets>0)
                        openBrackets--;
                }

                if (!skip)
                    sb.Append(c);

                bool isEoL = _eolChars.Contains(Current);
                if (isEoL
                    || (Current == '|' && _insideLabel)
                    || (Current == '=' && !openQuotes)
                    || (Current == ' ' && !openQuotes)
                    || (Current == ']' && openBrackets == 0)) // && !openQuotes // doesn't seem needed
                {
                    if (openBrackets > 0)
                        throw new ParseException(CurrentPosition, "Identifier ended without proper matching brackets.");
                    if (isEoL)
                    {
                        _insideTag = false;
                        _insideLabel = false;
                        if (_inlineTag && maxOpenBrackets > 0)
                            throw new ParseException(CurrentPosition,
                                "prematurely ended an inline tag containing an identifier with embedded brackets.");
                    }
                    return MakeToken(TokenType.Identifier, sb.ToString());
                }

                c = skip ? Skip() : Advance();
            }
            throw new ParseException(CurrentPosition, "ReadIdentifier exceeded maximum iterations.");
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
