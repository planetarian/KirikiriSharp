namespace KirikiriSharp.Lexer
{
    public enum TokenType
    {
        // Ignored by parser
        
        Invalid,

        WhiteSpace,
        LineEnd,

        Asterisk,
        Label,
        Identifier,

        Boolean,
        Integer,
        Float,

        Pipe,
        At,

        LeftBracket,
        RightBracket,

        /*

        LineComment,
        BlockComment,


        Comma,
        Colon,
        SemiColon,
        Question,
        Bang,

        String,
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
