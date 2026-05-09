namespace rec_file_lib.Query;

internal enum RecSelectionExpressionOperator
{
    Equals,
    NotEquals,
    Contains
}

internal sealed record class RecSelectionExpression(
    string FieldName,
    RecSelectionExpressionOperator Operator,
    string Value);
