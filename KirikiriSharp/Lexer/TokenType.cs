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

        Integer,
        Float,

        

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
        Boolean,

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
