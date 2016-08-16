namespace KirikiriSharp.Lexer
{
    public enum TokenType
    {
        // Ignored by parser

        Asterisk,
        Hash,
        LineComment,


        Invalid,

        WhiteSpace,
        LineEnd,

        Label,
        Identifier,

        Boolean,
        Integer,
        Float,

        Pipe,
        At,

        LeftBracket,
        RightBracket,
        
        Equals,
        Text,
        /*

        BlockComment,


        Comma,
        Colon,
        SemiColon,
        Question,
        Bang,

        Integer,
        Float,

        Global,
        Scene,
        Template,
        Group,
        Entity,
        Eval,

        Asterisk,
        Slash,
        Percent,
        Caret,
        Plus,
        Minus,

        LeftParen,
        RightParen,
        LeftBrace,
        RightBrace,
        */

        Eof
    }
}
