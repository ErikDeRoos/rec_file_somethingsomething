namespace rec_file_lib.Query;

internal static class RecSelectionExpressionParser
{
    public static RecSelectionExpression? Parse(string? expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            return null;
        }

        var trimmed = expression.Trim();

        var containsIndex = trimmed.IndexOf("~", StringComparison.Ordinal);
        if (containsIndex >= 0)
        {
            var fieldName = trimmed[..containsIndex].Trim();
            var valueToken = trimmed[(containsIndex + 1)..].Trim();
            return CreateExpression(fieldName, RecSelectionExpressionOperator.Contains, valueToken, expression);
        }

        var notEqualsIndex = trimmed.IndexOf("!=", StringComparison.Ordinal);
        if (notEqualsIndex >= 0)
        {
            var fieldName = trimmed[..notEqualsIndex].Trim();
            var valueToken = trimmed[(notEqualsIndex + 2)..].Trim();
            return CreateExpression(fieldName, RecSelectionExpressionOperator.NotEquals, valueToken, expression);
        }

        var equalsIndex = trimmed.IndexOf("=", StringComparison.Ordinal);
        if (equalsIndex >= 0)
        {
            var fieldName = trimmed[..equalsIndex].Trim();
            var valueToken = trimmed[(equalsIndex + 1)..].Trim();
            return CreateExpression(fieldName, RecSelectionExpressionOperator.Equals, valueToken, expression);
        }

        throw new FormatException($"Invalid selection expression: '{expression}'.");
    }

    private static RecSelectionExpression CreateExpression(
        string fieldName,
        RecSelectionExpressionOperator op,
        string valueToken,
        string originalExpression)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            throw new FormatException($"Invalid selection expression: '{originalExpression}'.");
        }

        var value = ParseValue(valueToken, originalExpression);
        return new RecSelectionExpression(fieldName, op, value);
    }

    private static string ParseValue(string token, string originalExpression)
    {
        if (token.Length >= 2 && token[0] == '"' && token[^1] == '"')
        {
            return token[1..^1];
        }

        if (token.Contains(' ', StringComparison.Ordinal))
        {
            throw new FormatException($"Invalid selection expression: '{originalExpression}'.");
        }

        return token;
    }
}
