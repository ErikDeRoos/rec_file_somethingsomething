namespace rec_file_lib.Query;

internal enum RecSelectionExpressionOperator
{
    Equals,
    NotEquals,
    Contains,
    LessThan,
    LessThanOrEqual,
    GreaterThan,
    GreaterThanOrEqual
}

internal enum RecSelectionUnaryOperator
{
    Not
}

internal enum RecSelectionBinaryOperator
{
    And,
    Or
}

internal abstract record class RecSelectionExpression;

internal sealed record class RecSelectionComparisonExpression(
    string FieldName,
    RecSelectionExpressionOperator Operator,
    string Value) : RecSelectionExpression;

internal sealed record class RecSelectionUnaryExpression(
    RecSelectionUnaryOperator Operator,
    RecSelectionExpression Operand) : RecSelectionExpression;

internal sealed record class RecSelectionBinaryExpression(
    RecSelectionExpression Left,
    RecSelectionBinaryOperator Operator,
    RecSelectionExpression Right) : RecSelectionExpression;
