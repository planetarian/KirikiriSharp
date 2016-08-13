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

        OpenBracket,
        CloseBracket,

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
        At,

        LeftParen,
        RightParen,
        LeftBrace,
        RightBrace,
        */

        Eof
    }
}
