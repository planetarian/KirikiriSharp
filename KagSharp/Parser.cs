using System;
using System.Collections.Generic;
using KirikiriSharp.Expressions;
using KirikiriSharp.Lexer;
using KirikiriSharp.Parselets;

namespace KirikiriSharp
{
    public abstract class Parser
    {
        private readonly ITokenReader _reader;
        private readonly List<Token> _readTokens = new List<Token>();
        private readonly Dictionary<TokenType, IPrefixParselet> _prefixParselets
            = new Dictionary<TokenType, IPrefixParselet>();
        private readonly Dictionary<TokenType, INonPrefixParselet> _infixParselets
            = new Dictionary<TokenType, INonPrefixParselet>();
        private readonly Dictionary<TokenType, Dictionary<Type, IPrefixParselet>> _contextPrefixParselets
            = new Dictionary<TokenType, Dictionary<Type, IPrefixParselet>>();
        private readonly Dictionary<TokenType, Dictionary<Type, INonPrefixParselet>> _contextInfixParselets
            = new Dictionary<TokenType, Dictionary<Type, INonPrefixParselet>>();



        protected Parser(ITokenReader reader)
        {
            _reader = reader;
        }

        public void Register(TokenType tokenType, IPrefixParselet parselet, params Type[] types)
        {
            if (types.Length > 0)
            {
                if (!_contextPrefixParselets.ContainsKey(tokenType))
                    _contextPrefixParselets.Add(tokenType, new Dictionary<Type, IPrefixParselet>());
                foreach (Type type in types)
                    _contextPrefixParselets[tokenType].Add(type, parselet);
            }
            else
                _prefixParselets.Add(tokenType, parselet);
        }

        public void Register(TokenType tokenType, INonPrefixParselet parselet, params Type[] types)
        {
            if (types.Length > 0)
            {
                if (!_contextInfixParselets.ContainsKey(tokenType))
                    _contextInfixParselets.Add(tokenType, new Dictionary<Type, INonPrefixParselet>());
                foreach (Type type in types)
                    _contextInfixParselets[tokenType].Add(type, parselet);
            }
            else
                _infixParselets.Add(tokenType, parselet);
        }

        public IExpression ParseExpression()
        {
            return ParseExpression(0);
        }

        public IExpression ParseExpression(int precedence)
        {
            return ParseExpression<IExpression>(precedence);
        }

        public IExpression ParseExpression<TParent>() where TParent : IExpression
        {
            return ParseExpression<TParent>(0);
        }

        public IExpression ParseExpression<TParent>(int precedence) where TParent : IExpression
        {
            /*
            if (parentExpressionType != null && !typeof(IExpression).IsAssignableFrom(parentExpressionType))
                throw new InvalidOperationException(
                    "Parse: Parent expression type must be an IExpression. Unexpected " + parentExpressionType);
            */

            Token token = Consume();
            Type type = typeof(TParent);

            if (token.Type == TokenType.Eof)
                return new EofExpression();

            IPrefixParselet prefix = _prefixParselets[token.Type];

            if (type != typeof(IExpression) && _contextPrefixParselets.ContainsKey(token.Type) &&
                _contextPrefixParselets[token.Type].ContainsKey(type))
            {
                prefix = _contextPrefixParselets[token.Type][type];
            }


            if (prefix == null)
                throw new ParseException(token.Position, "Could not parse \"" + token.Text + "\".");

            IExpression left = prefix.Parse(this, token);

            int compPrecedence = GetPrecedence(type);

            while (precedence < compPrecedence)
            {
                token = LookAhead();

                INonPrefixParselet infix;
                if (type != typeof(IExpression) && _contextInfixParselets.ContainsKey(token.Type) &&
                        _contextInfixParselets[token.Type].ContainsKey(type))
                    infix = _contextInfixParselets[token.Type][type];
                else
                    infix = _infixParselets[token.Type];

                if (infix.ConsumeToken)
                    Consume();

                left = infix.Parse(this, left, token);

                compPrecedence = GetPrecedence(type);
            }

            return left;
        }

        private int GetPrecedence(Type parentExpressionType)
        {
            TokenType type = LookAhead().Type;
            if (_contextInfixParselets.ContainsKey(type)
                && _contextInfixParselets[type].ContainsKey(parentExpressionType))
                return _contextInfixParselets[type][parentExpressionType].Precedence;

            return _infixParselets.ContainsKey(type) ? _infixParselets[type].Precedence : 0;
        }

        public bool Match(TokenType expected)
        {
            Token token = LookAhead();
            if (token.Type != expected)
                return false;
            Consume();
            return true;
        }

        public Token Consume(TokenType expected)
        {
            Token token = LookAhead();
            if (token.Type != expected)
                throw new InvalidOperationException(
                    "Expected token " + expected + " and found " + token.Type);
            return Consume();
        }

        public Token Consume()
        {
            LookAhead();
            Token read = _readTokens[0];
            _readTokens.RemoveAt(0);
            return read;
        }

        public Token LookAhead(int distance = 0)
        {
            // read in as many as needed.
            while (distance >= _readTokens.Count)
            {
                _readTokens.Add(_reader.ReadToken());
            }
            // Get the queued token.
            return _readTokens[distance];
        }


        /*
        private ITokenReader reader;
        private List<Token> read;
        private List<Token> consumed;

        public PositionSpan Span
        {
            get { return new PositionSpan(this, Last(1).Position); }
        }

        public Token Current
        {
            get { return LookAhead(0); }
        }


        protected Parser(ITokenReader reader)
        {
            Expect.NotNull(reader);

            this.reader = reader;
            read = new List<Token>();
            consumed = new List<Token>();
        }

        public Token Last(int offset)
        {
            Expect.PositiveNonZero(offset);
            return consumed[offset - 1];
        }

        public bool LookAhead (params TokenType[] tokens)
        {
            for (int i = 0; i < tokens.Length; i++)
            {
                if (!LookAhead(i).IsType(tokens[i])) return false;
            }
            return true;
        }

        public bool LookAheadAny(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (LookAhead(type)) return true;
            }
            return false;
        }

        public bool Match(params TokenType[] types)
        {
            // see if they match
            if (!LookAhead(types)) return false;
            // consume the matched tokens
            for (int i = 0; i < types.Length; i++)
            {
                Consume();
            }

            return true;
        }

        public bool MatchAny(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (Match(type)) return true;
            }
            return false;
        }

        public Token Consume()
        {
            // Make sure we've read the token.
            LookAhead(0);

            consumed.Add(read[0]);
            read.RemoveAt(0);
            return Last(1);
        }

        public Token Consume(TokenType type)
        {
            if (Match(type))
                return Last(1);
            Token current = Current;
            String message = String.Format("Expected token {0}, found {1}", type, current);
            throw new ParseException(current.Position, message);
        }

        private Token LookAhead(int distance)
        {
            // read in as many as needed.
            while (distance >= read.Count)
            {
                read.Add(reader.ReadToken());
            }
            // get the queued token.
            return read[distance];
        }
        */
    }
}
