using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using KagSharp.Expressions;
using KagSharp.Lexer;
using KagSharp.Parselets;

namespace KagSharp
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

            IPrefixParselet prefix;
            try
            {
                prefix = _prefixParselets[token.Type];
            }
            catch (Exception ex)
            {
                throw new ParseException(token.Position,
                    $"Token type {token.Type} not registered in prefixParselets.", ex);
            }

            if (type != typeof(IExpression) && _contextPrefixParselets.ContainsKey(token.Type) &&
                _contextPrefixParselets[token.Type].ContainsKey(type))
            {
                try
                {
                    prefix = _contextPrefixParselets[token.Type][type];
                }
                catch (Exception ex)
                {
                    throw new ParseException(token.Position,
                        $"Type {type} not registered at contextPrefixParselets[{token.Type}].", ex);
                }
            }


            if (prefix == null)
                throw new ParseException(token.Position, $"Could not parse \"{token.Text}\".");

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

        
    }
}
