namespace KagSharp.Expressions
{
    public interface IExpressionVisitor<out TR>
    {
        TR Visit(EofExpression expression, string context);
        TR Visit(EndOfLineExpression expression, string context);
        TR Visit(DocumentExpression expression, string context);
        TR Visit(LabelExpression expression, string context);
        TR Visit(IdentifierExpression expression, string context);
        TR Visit(TextExpression expression, string context);
        TR Visit(ParameterExpression expression, string context);
        TR Visit(TitleExpression expression, string context);
        TR Visit(TagExpression expression, string context);
        TR Visit(MacroExpression expression, string context);
        TR Visit(IfExpression expression, string context);
        TR Visit(IScriptExpression expression, string context);
        /*
        TR Visit(AssignExpression expression, string context);
        TR Visit(BoolExpression expression, string context);
        TR Visit(ComponentExpression expression, string context);
        TR Visit(CompoundValueExpression expression, string context);
        TR Visit(DocumentExpression expression, string context);
        TR Visit(EntityBaseExpression expression, string context);
        TR Visit(EntityBodyExpression expression, string context);
        TR Visit(EntityExpression expression, string context);
        TR Visit(EvalExpression expression, string context);
        TR Visit(FloatExpression expression, string context);
        TR Visit(FunctionExpression expression, string context);
        TR Visit(GlobalExpression expression, string context);
        TR Visit(GroupExpression expression, string context);
        TR Visit(IdentifierExpression expression, string context);
        TR Visit(IntegerExpression expression, string context);
        TR Visit(ListExpression expression, string context);
        TR Visit(OperatorExpression expression, string context);
        TR Visit(PostfixExpression expression, string context);
        TR Visit(PrefixExpression expression, string context);
        TR Visit(SceneExpression expression, string context);
        TR Visit(StringExpression expression, string context);
        TR Visit(TemplateExpression expression, string context);
        TR Visit(TernaryExpression expression, string context);
        */
    }
}
