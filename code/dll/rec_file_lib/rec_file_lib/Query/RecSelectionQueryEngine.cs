using rec_file_lib.Model;

namespace rec_file_lib.Query;

internal sealed class RecSelectionQueryEngine
{
    public RecRecordSet? Select(RecRecordSet? recordSet, RecSelectionQueryOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (recordSet is null)
        {
            return null;
        }

        var expression = RecSelectionExpressionParser.Parse(options.Expression);
        var selectedRecords = new List<RecRecord>();

        for (var recordIndex = 0; recordIndex < recordSet.Records.Count; recordIndex++)
        {
            if (options.SelectedIndexes is not null && !options.SelectedIndexes.Contains(recordIndex))
            {
                continue;
            }

            var record = recordSet.Records[recordIndex];
            if (!MatchesQuickFilter(record, options.QuickFilter))
            {
                continue;
            }

            if (!MatchesExpression(record, expression))
            {
                continue;
            }

            selectedRecords.Add(ProjectRecord(record, options.ProjectedFields));
        }

        return recordSet with { Records = selectedRecords.ToArray() };
    }

    private static bool MatchesQuickFilter(RecRecord record, string? quickFilter)
    {
        if (string.IsNullOrEmpty(quickFilter))
        {
            return true;
        }

        foreach (var field in record.Fields)
        {
            if (field.Value.Contains(quickFilter, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static bool MatchesExpression(RecRecord record, RecSelectionExpression? expression)
    {
        if (expression is null)
        {
            return true;
        }

        foreach (var field in record.Fields)
        {
            if (!string.Equals(field.Name, expression.FieldName, StringComparison.Ordinal))
            {
                continue;
            }

            return expression.Operator switch
            {
                RecSelectionExpressionOperator.Equals => string.Equals(field.Value, expression.Value, StringComparison.Ordinal),
                RecSelectionExpressionOperator.NotEquals => !string.Equals(field.Value, expression.Value, StringComparison.Ordinal),
                RecSelectionExpressionOperator.Contains => field.Value.Contains(expression.Value, StringComparison.Ordinal),
                _ => false
            };
        }

        return false;
    }

    private static RecRecord ProjectRecord(RecRecord record, IReadOnlySet<string>? projectedFields)
    {
        if (projectedFields is null)
        {
            return record;
        }

        var fields = record.Fields
            .Where(field => projectedFields.Contains(field.Name))
            .ToArray();

        return record with { Fields = fields };
    }
}
